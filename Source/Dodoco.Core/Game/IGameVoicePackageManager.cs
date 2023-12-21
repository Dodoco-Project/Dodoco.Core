namespace Dodoco.Core.Game;

using Dodoco.Core.Protocol.Company.Launcher.Resource;

public interface IGameVoicePackageManager {

    /// <summary>
    /// Returns a list with all installed voices packages' languages for current game installation.
    /// To achieve this, the method simply verify if each supported language from <see cref="F:Dodoco.Core.Game.GameLanguage.All"/> does have a 
    /// folder named after it inside "*_Data/StreamingAssets/AudioAssets" directory.
    /// </summary>
    /// <returns>
    /// A list containing all installed voices packages' languages for current game installation.
    /// </returns>
    IEnumerable<GameLanguage> GetInstalledVoices();

    Task<ResourceVoicePack> GetVoicePackageUpdateAsync(ResourceDiff resourceDiff, GameLanguage language);

    /// <summary>
    /// Installs the given language's voice package to the game installation.
    /// </summary>
    Task InstallVoicePackageAsync(GameLanguage language, bool forceReinstall, ProgressReporter<ProgressReport>? reporter, CancellationToken token = default);

    Task UpdateVoicePackageAsync(IGameUpdateManager updateManager, IGameIntegrityManager integrityManager, GameLanguage language, ProgressReporter<ProgressReport>? reporter, CancellationToken token = default);

    void RemoveVoicePackage(GameLanguage language);

}