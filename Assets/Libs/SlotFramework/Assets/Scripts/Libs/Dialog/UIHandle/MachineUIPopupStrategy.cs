namespace Libs
{
    public class MachineUIPopupStrategy:IUIPopupStrategy
    {
        public bool CanPopup()
        {
            return BaseGameConsole.ActiveGameConsole().IsInSlotMachine();
        }
    }
}