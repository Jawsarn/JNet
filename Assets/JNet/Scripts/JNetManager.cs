using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace JNetInternal
{
    public class JNetManager : MonoBehaviour
    {
        private static JNetManager p_singleton;
        public static JNetManager m_singleton
        {
            get
            {
                // If you write bad code, you don't get second chances
                if (p_singleton == null)
                {
                    Debug.LogError("No instance found of SteamManager, things will crash.");
                    return null;
                }
                else
                {
                    return p_singleton;
                }
            }
        }

        public enum ConnectionState
        {
            DISCONNECTED,
            CONNECTING,
            CONNECTED,
        }

        [SerializeField]
        ConnectionState m_lobbyConnectionState = ConnectionState.DISCONNECTED;

        // Considered using Resource.Load(), but requires sending strings over network
        ///<summary>
        /// Prefabs that are spawnable during runtime, note: needs to be modified to be changed during runtime (use some function to add to dict as well)
        ///</summary>
        public List<GameObject> m_networkPrefabs = new List<GameObject>();
        Dictionary<string, GameObject> m_networkPrefabsDict = new Dictionary<string, GameObject>();

        // TODO consider making this a #define, shouldn't change runtime?
        public int m_defaultMembers = 4; // Default members if no input
        public bool m_hasConstantSetMembers = false; // If true, m_defaultMembers will override input
        public int m_maximumMembersByUser = 8; // Does nothing if hasConstantSetMembers is true

        [Header("Packet/Messaging settings")]
        public EP2PSend[] m_channels = { EP2PSend.k_EP2PSendReliable, EP2PSend.k_EP2PSendUnreliable };
        public uint m_maxPacketPerChannelPerUpdate = 100; // Since we're using normal update, this can freeze the game

        CSteamID m_currentLobby = CSteamID.Nil;
        CSteamID m_currentHostID = CSteamID.Nil;
        uint m_clientIndex = 0;
        bool m_isHost = false; // Used for checking if we enteredLobby as creators or not


        // This creates connection to all clients for a nonMaster client
        // i.e. data such as position and rotation doesn't have to bypass masterClient
        public bool m_useDirectConenctions = false;

        // callbacks
        private Callback<LobbyEnter_t> m_LobbyEntered;
        private CallResult<LobbyMatchList_t> m_LobbyMatchList;


        // logical callbacks
        JNetInternalMessageLogic m_msgLogic = new JNetInternalMessageLogic();

        // TODO add debug print levels and debug prints

        private void Awake()
        {
            // Only one instance of SteamManager at a time!
            if (p_singleton != null)
            {
                Debug.LogError("Multiple steamManagers");
                Destroy(gameObject);
                return;
            }
            p_singleton = this;

            // Add callbacks
            if (SteamInit.m_InitializationSucceeded)
            {
                m_LobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
                m_LobbyMatchList = CallResult<LobbyMatchList_t>.Create(OnLobbyMatchList);
            }

            // Register callbacks for messages
            m_msgLogic.RegisterCallbacks();


            List<GameObject> m_errorObjects = new List<GameObject>();
            foreach (var item in m_networkPrefabs)
            {
                if (item.GetComponent<JNetIdentity>() == null)
                {
                    // TODO make this whole thing OnGUI or mby add it under verbose stuff logging?
                    Debug.LogError("ERROR: Spawnable prefab without gameObject found name:" + item.name);
                }
            }

            // Removing objects that didn't have netIDs so other things can work
            for (int i = 0; i < m_errorObjects.Count; i++)
            {
                m_networkPrefabs.Remove(m_errorObjects[i]);
            }

            // Add prefabs to string cache
            foreach (var pref in m_networkPrefabs)
            {
                m_networkPrefabsDict.Add(pref.name, pref);
            }
        }

        public ulong GetCurrentHostID()
        {
            return m_currentHostID.m_SteamID;
        }
        
        public ulong GetClientID()
        {
            return SteamUser.GetSteamID().m_SteamID;
        }

        internal uint GetCurrentClientIndex()
        {
            return m_clientIndex;
        }

        public GameObject GetPrefab(GameObject prefabObj)
        {
            if(m_networkPrefabs.Contains(prefabObj))
            {
                return prefabObj;
            }
            else
            {
                return null;
            }
            //return GetPrefab(prefabObj.name); // TODO consider faster?
        }

        public GameObject GetPrefab(string prefabName)
        {
            return m_networkPrefabsDict[prefabName];
        }

        public ushort GetPrefabIndex(GameObject prefab)
        {
            // Not safe
            return (ushort)m_networkPrefabs.IndexOf(prefab);
        }

        public GameObject InstanciateFromPrefab(ushort prefabID, Vector3 position, Quaternion rotation)
        {
            if (prefabID < m_networkPrefabs.Count)
            {
                return (GameObject)Instantiate(m_networkPrefabs[prefabID].gameObject, position, rotation);
            }
            
            return null;
        }

        public void JoinLobby(CSteamID lobbyId)
        {
            if (!SteamInit.m_InitializationSucceeded)
            {
                m_lobbyConnectionState = ConnectionState.DISCONNECTED;

                // TODO add error message or similar

                return;
            }

            if (m_lobbyConnectionState != ConnectionState.DISCONNECTED)
            {
                // We are already in a connection phase.. should be disconnected first?
                // TODO check if this is true
                return;
            }

            m_lobbyConnectionState = ConnectionState.CONNECTING;
            m_isHost = false;
            SteamMatchmaking.JoinLobby((CSteamID)lobbyId);

            // ...continued in OnLobbyEntered callback
        }

        public void JoinRandomLobby()
        {
            if (!SteamInit.m_InitializationSucceeded)
            {
                m_lobbyConnectionState = ConnectionState.DISCONNECTED;

                // TODO add error message or similar

                return;
            }

            if (m_lobbyConnectionState != ConnectionState.DISCONNECTED)
            {
                // We are already in a connection phase.. should be disconnected first?
                // TODO check if this is true
                return;
            }

            m_lobbyConnectionState = ConnectionState.CONNECTING;
            m_isHost = false;

            SteamMatchmaking.AddRequestLobbyListStringFilter("Version", GameVersion.m_version, ELobbyComparison.k_ELobbyComparisonEqual);
            var call = SteamMatchmaking.RequestLobbyList();
            m_LobbyMatchList.Set(call);

            // ...continued in OnLobbyMatchList callback
        }

        void OnLobbyMatchList(LobbyMatchList_t pCallback, bool bIOFailure)
        {
            // TODO don't hardcode this to random join
            uint numLobbies = pCallback.m_nLobbiesMatching;

            if (numLobbies == 0)
            {
                Debug.Log("No lobbies found");
                return;
            }

            if(numLobbies > 1)
            {
                Debug.Log("Found more then one lobby! (not good if testing)");
            }

            Debug.Log("Joining lobby");
            JoinLobby(SteamMatchmaking.GetLobbyByIndex(0));
        }

        public void CreateLobby(ELobbyType lobbyType)
        {
            CreateLobby(lobbyType, m_defaultMembers);
        }

        public void CreateLobby(ELobbyType lobbyType, int maxMembers)
        {
            if (!SteamInit.m_InitializationSucceeded)
            {
                m_lobbyConnectionState = ConnectionState.DISCONNECTED;

                // TODO add error message or similar

                return;
            }

            if (m_lobbyConnectionState != ConnectionState.DISCONNECTED)
            {
                // We are already in a connection phase.. should be disconnected first?
                // TODO check if this is true
                return;
            }

            m_lobbyConnectionState = ConnectionState.CONNECTING;

            // Check member rules
            if (m_hasConstantSetMembers)
            {
                maxMembers = m_defaultMembers;
            }
            else
            {
                maxMembers = Mathf.Min(maxMembers, m_maximumMembersByUser);
            }

            m_isHost = true;
            SteamMatchmaking.CreateLobby(lobbyType, maxMembers);
            // ...continued in OnLobbyEntered callback
        }

        void OnLobbyEntered(LobbyEnter_t pCallback)
        {
            // If steam not running or we didn't manage to enter lobby
            if (!SteamInit.m_InitializationSucceeded || pCallback.m_EChatRoomEnterResponse != (uint)EChatRoomEnterResponse.k_EChatRoomEnterResponseSuccess)
            {
                m_lobbyConnectionState = ConnectionState.DISCONNECTED;

                // TODO add message/callback for different errors

                return;
            }

            m_lobbyConnectionState = ConnectionState.CONNECTED;
            m_currentLobby = new CSteamID(pCallback.m_ulSteamIDLobby);


            // Since both creating and joining lobby uses same callback, we use a boolean to check
            if (m_isHost)
            {
                SetupHosting();
            }
            else
            {
                var hostUserId = SteamMatchmaking.GetLobbyOwner(m_currentLobby);
                var me = SteamUser.GetSteamID();

                // If we became the lobbyhost, the host left, and we disconnect for now
                // TODO check if we want to be hosts instead
                if (hostUserId.m_SteamID == me.m_SteamID)
                {
                    // TODO disconnect here
                    Debug.LogError("Not implemented");
                    return;
                }

                // joined friend's lobby.
                RequestP2PConnectionWithHost();
            }
        }

        void SetupHosting()
        {
            // Set version for filterings
            SteamMatchmaking.SetLobbyData(m_currentLobby, "Version", GameVersion.m_version);

            // TODO Register handlers for connect and spawn messages

            // TODO setup host topology?
            // TODO setup connection locally, => make messges go to ourselves as well, not sure if needed

            // TODO Add callback to tell we are ready
        }

        void RequestP2PConnectionWithHost()
        {
            m_currentHostID = SteamMatchmaking.GetLobbyOwner(m_currentLobby);

            // Send packet to request connection to host via Steam's NAT punch or relay servers
            SteamNetworking.SendP2PPacket(m_currentHostID, null, 0, EP2PSend.k_EP2PSendReliable);

            /**
             * Not sure if we should seperate this into a function of loop incomming and check on connectionState in the regular loop
             * If we have it here we can ignore whole loop of other messages, might be easier? But this method needs to be monitized/destroyed from other functions if things happen
             * If we have it in update loop we need to check all other messages vs P2PRequest
             * Might consider not making this an empty message.
             * */

        }

        void CheckForP2PAcceptMessageFromHost()
        {
            uint msgSize;
            if (SteamNetworking.IsP2PPacketAvailable(out msgSize))
            {
                CSteamID senderID;
                byte[] data = new byte[msgSize];
                if (SteamNetworking.ReadP2PPacket(data, msgSize, out msgSize, out senderID))
                {
                    // This can change later, but shouldn't happen now
                    if (msgSize != 0)
                    {
                        return;
                    }
                    // If we received from nonhost, 
                    if (senderID != m_currentHostID)
                    {
                        return;
                    }
                    // TODO add something it contains?

                    m_lobbyConnectionState = ConnectionState.CONNECTED;
                }
                else
                {
                    // TODO add error message
                }
            }
        }

        void Update()
        {
            if (!SteamInit.m_InitializationSucceeded)
            {
                return;
            }

            // TODO Check if we need to Empty packages if we are disconnected
            // Might've been a bug caused by other things in other project

            if (m_lobbyConnectionState == ConnectionState.DISCONNECTED)
            {
                return;
            }
            else if (m_lobbyConnectionState == ConnectionState.CONNECTING)
            {
                CheckForP2PAcceptMessageFromHost();

                // Might've changed state and received updates
                if (m_lobbyConnectionState != ConnectionState.CONNECTED)
                {
                    return;
                }
            }

            // Process messages
            JNetPacketHandler.ProcessIncommingPackets();
        }

        void LateUpdate()
        {
            if (!SteamInit.m_InitializationSucceeded)
            {
                return;
            }

            if (m_lobbyConnectionState != ConnectionState.CONNECTED)
            {
                return;
            }

            // Process messages
            JNetPacketHandler.ProcessOutgoingPackets();
        }
    }
}