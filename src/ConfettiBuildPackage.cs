global using Community.VisualStudio.Toolkit;
global using Microsoft.VisualStudio.Shell;
global using System;
global using Task = System.Threading.Tasks.Task;
using LottieSharp;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Media;
using System.Windows;
using Microsoft.VisualStudio.Shell.Interop;

namespace ConfettiBuild
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration(Vsix.Name, Vsix.Description, Vsix.Version)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(PackageGuids.ConfettiBuildString)]
    [ProvideOptionPage(typeof(OptionsProvider.GeneralOptions), "ConfettiBuild", "General", 0, 0, true)]
    [ProvideProfile(typeof(OptionsProvider.GeneralOptions), "ConfettiBuild", "General", 0, 0, true)]
    [ProvideAutoLoad(UIContextGuids80.NoSolution, PackageAutoLoadFlags.BackgroundLoad)]
    public sealed class ConfettiBuildPackage : ToolkitPackage
    {
        EnvDTE.DTE _dte;

        Window _animationWindow;
        LottieAnimationView _animationView;
        DateTime _buildStart;

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            if (!(await GetServiceAsync(typeof(EnvDTE.DTE)) is EnvDTE.DTE dte))
                return;

            _dte = dte;

            _dte.Events.BuildEvents.OnBuildBegin += BuildEvents_OnBuildBegin;
            _dte.Events.BuildEvents.OnBuildDone += BuildEvents_OnBuildDone;

            PrepareAnimationWindow();
        }

        private void BuildEvents_OnBuildBegin(EnvDTE.vsBuildScope Scope, EnvDTE.vsBuildAction Action)
        {
            _buildStart = DateTime.Now;
            System.Diagnostics.Debug.WriteLine($"OnBuildBegin({_buildStart})");
        }

        private void BuildEvents_OnBuildDone(EnvDTE.vsBuildScope Scope, EnvDTE.vsBuildAction Action)
        {
            var elapsed = (DateTime.Now - _buildStart).TotalSeconds;
            System.Diagnostics.Debug.WriteLine($"OnBuildDone({elapsed} seconds) MinBuildDuration:{General.Instance.MinBuildDuration} secs");

            if (elapsed >= General.Instance.MinBuildDuration)
            {
                if (Action == EnvDTE.vsBuildAction.vsBuildActionBuild || Action == EnvDTE.vsBuildAction.vsBuildActionRebuildAll)
                {
                    ShowAnimation();
                }
            }
        }

        private void PrepareAnimationWindow()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            _animationView = new LottieAnimationView
            {
                AutoPlay = false,
                FileName = $@".\Animations\{General.Instance.Animation.ToString().ToLowerInvariant()}.json",
                Background = Brushes.Transparent,
                RepeatCount = 0,
            };

            _animationWindow = new Window
            {
                Title = "Confetti",
                VerticalContentAlignment = VerticalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                AllowsTransparency = true,
                WindowStyle = WindowStyle.None,
                WindowState = WindowState.Maximized,
                Background = Brushes.Transparent,
                Content = _animationView,
                Topmost = true,
            };

            _animationWindow.Show();
        }

        private void ShowAnimation()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            System.Diagnostics.Debug.WriteLine("Showing animation...");
            _animationView.PlayAnimation();
        }
    }
}