using System.ComponentModel;
using System.Runtime.InteropServices;

namespace ConfettiBuild
{
    internal partial class OptionsProvider
    {
        // Register the options with this attribute on your package class:
        // [ProvideOptionPage(typeof(OptionsProvider.GeneralOptions), "ConfettiBuild", "General", 0, 0, true, SupportsProfiles = true)]
        [ComVisible(true)]
        public class GeneralOptions : BaseOptionPage<General> { }
    }

    public class General : BaseOptionModel<General>
    {
        private int _minBuildDuration = 1;

        [Category("General")]
        [DisplayName("Minimum build duration (sec)")]
        [Description("Show animation only if build duration is more than the specified value in second.")]
        [DefaultValue(1)]
        public int MinBuildDuration
        {
            get => _minBuildDuration;
            set
            {
                _minBuildDuration = Math.Max(0, value);
            }
        }

        [Category("General")]
        [DisplayName("Animation")]
        [Description("Select the animation to play (restart required).")]
        [DefaultValue(Animation.Confetti1)]
        public Animation Animation { get; set; }
    }

    public enum Animation
    {
        Confetti1,

        Confetti2,

        Confetti3
    }
}
