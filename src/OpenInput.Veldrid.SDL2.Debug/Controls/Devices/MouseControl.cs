namespace OpenInput.Debug.Controls.Devices
{
    using ImGuiNET;
    using OpenInput.Mechanics;
    using OpenInput.Mechanics.Input;

    /// <summary>
    /// Debug Control for getting a insight into a mouse state.
    /// </summary>
    public class MouseControl : Control
    {
        private readonly StringBuilder sb = new StringBuilder();

        public MouseControl(IMouse mouse)
        {
            this.Mouse = mouse;
        }

        public IMouse Mouse { get; set; }

        public override void DrawControl()
        {
            ImGui.Text($"Mouse (Name: \"{this.Mouse.Name}\")");

            var mouseState = inputContext.Mouse.GetCurrentState();

            sb.Clear();
            sb.AppendLine($"Position: { mouseState.X }, { mouseState.Y }");
            sb.AppendLine($"MouseWheel: { mouseState.ScrollWheelValue }");
            sb.AppendLine();
            sb.AppendLine($"Left Button: { mouseState.LeftButton }");
            sb.AppendLine($"Middle Button: { mouseState.MiddleButton }");
            sb.AppendLine($"Right Button: { mouseState.RightButton }");
            sb.AppendLine($"XButton1: { mouseState.XButton1 }");
            sb.AppendLine($"XButton2: { mouseState.XButton2 }");

            ImGui.Text(sb.ToString());
        }
    }
}