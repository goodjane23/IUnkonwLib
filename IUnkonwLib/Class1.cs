using Ascon.Plm.Loodsman.PluginSDK;
using IUnkonwLib.Views;
using System;

namespace IUnkonwLib
{
    [LoodsmanPlugin]
    public class Class1 : ILoodsmanNetPlugin
    {
        public void BindMenu(IMenuDefinition menu)
        {
            menu.AddMenuItem("Игра", TestMethod, CheckTrue);
        }

        private bool CheckTrue(INetPluginCall call)
        {
            return true;
        }

        private void TestMethod(INetPluginCall call)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.ShowDialog();
        }

        #region NotImplemented

        public void OnCloseDb()
        {

        }

        public void OnConnectToDb(INetPluginCall call)
        {
        }

        public void PluginLoad()
        {
            
        }

        public void PluginUnload()
        {
        }
        #endregion
    }
}
