namespace Libs
{
    public class SystemUIPopupStrategy:IUIPopupStrategy
    {
        public virtual bool CanPopup()
        {
            return BaseGameConsole.ActiveGameConsole().IsInSlotMachine()||BaseGameConsole.ActiveGameConsole().IsInLobby();
        }
    }
}