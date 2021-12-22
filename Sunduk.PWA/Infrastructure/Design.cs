using MudBlazor;

namespace Sunduk.PWA.Infrastructure
{
    public static class Design
    {
        public static string GetFontSize(this Breakpoint breakpoint)
        {
            switch (breakpoint)
            {
                case Breakpoint.Xs: return "xx-small";
                case Breakpoint.Sm: return "x-small";
                default: return "medium";
            }
        }

        public static string GetPaddingRight(this Breakpoint breakpoint)
        {
            switch (breakpoint)
            {
                case Breakpoint.Xs: return "10px";
                case Breakpoint.Sm: return "15px";
                default: return "25px";
            }
        }
    }
}
