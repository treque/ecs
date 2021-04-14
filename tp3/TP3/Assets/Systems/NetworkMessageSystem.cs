public class NetworkMessageSystem : ISystem
{
    public string Name
    {
        get
        {
            return GetType().Name;
        }
    }

    // In charge of sending all messages pending sending
    public void UpdateSystem()
    {
        bool messagingInfoFound = ComponentsManager.Instance.TryGetComponent(new EntityComponent(0), out MessagingInfo messagingInfo);

        if (!messagingInfoFound)
        {
            messagingInfo = new MessagingInfo() { currentMessageId = 0,
                                                currentInputMessageId = 0,
                                                currentServerAckMessageId = 0 };
        }

        if (ECSManager.Instance.NetworkManager.isServer)
        {
            // Ceci va envoyer les messages
            ComponentsManager.Instance.ForEach<ReplicationMessage>((entityID, msg) =>
            {
                msg.messageID = messagingInfo.currentMessageId++;
                ECSManager.Instance.NetworkManager.SendReplicationMessage(msg);
            });

            ComponentsManager.Instance.ForEach<ServerAcknowledgeMessage>((entityID, msg) =>
            {
                msg.ackMessageID = messagingInfo.currentServerAckMessageId++;
                ECSManager.Instance.NetworkManager.SendServerAcknowledgementMessage(msg);
            });
            
            ComponentsManager.Instance.ClearComponents<ServerAcknowledgeMessage>();
        }

        if (ECSManager.Instance.NetworkManager.isClient)
        {  
            //TODO
            /*ComponentsManager.Instance.ForEach<ReplicationMessage>((entityID, msg) =>
            {
                msg.messageID = messagingInfo.currentMessageId++;
                ECSManager.Instance.NetworkManager.SendClientReplicationMessage(msg);
            });*/

            ComponentsManager.Instance.ForEach<InputMessage>((entityID, msg) =>
            {
                msg.inputMessageID = messagingInfo.currentInputMessageId++;
                ECSManager.Instance.NetworkManager.SendClientInputMessage(msg);
            });
            
            ComponentsManager.Instance.ClearComponents<InputMessage>();
        }

        ComponentsManager.Instance.SetComponent<MessagingInfo>(new EntityComponent(0), messagingInfo);
    }
}