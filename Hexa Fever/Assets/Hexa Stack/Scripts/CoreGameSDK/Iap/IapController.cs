namespace Hexa_Stack.Scripts.CoreGameSDK.Iap
{
    public class IapController
    {
        private static IapController instance;
        private readonly IapNativeBridge iapNativeBridge;

        private IapController()
        {
            iapNativeBridge = new IapNativeBridge();
        }

        public static IapController GetInstance()
        {
            return instance ??= new IapController();
        }

        public void SendPurchaseInfo(double dollarValue, string currency)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            iapNativeBridge.sendPurchaseInfo(dollarValue, currency);
#endif
        }
    }
}