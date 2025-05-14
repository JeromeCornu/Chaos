using Mirror;

namespace DefaultNamespace
{
    public class ClientToServerComands : NetworkBehaviour
    {
        public void NotifyServerReady()
        {
            if (isLocalPlayer)
                CmdPlayerSceneReady();
        }

        [Command]
        private void CmdPlayerSceneReady()
        {
            GameSync.Instance.PlayerReady(connectionToClient);
        }
    }
}