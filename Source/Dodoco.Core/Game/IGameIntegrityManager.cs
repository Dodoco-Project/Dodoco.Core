namespace Dodoco.Core.Game;

public interface IGameIntegrityManager: IStatefulEntity<GameIntegrityManagerState> {

    /// <inheritdoc cref="GetInstallationIntegrityReportAsync(System.Collections.Generic.List{Dodoco.Core.Game.GamePkgVersionEntry}, Dodoco.Core.ProgressReporter{ProgressReport}?, System.Threading.CancellationToken)"/>
    Task<List<GameFileIntegrityReport>> GetInstallationIntegrityReportAsync(List<GamePkgVersionEntry> entries);
    
    /// <summary>
    /// Verify all game's files searching for either missing files or files whose checksum diffears
    /// from the one in pkg_version.
    /// </summary>
    /// <returns>
    /// Returns a <see cref="System.Collections.Generic.List{T}(Dodoco.Core.Game.GameFileIntegrityReport)"/>
    /// referencing the missing files or files whose checksum diffears
    /// from the one in pkg_version.
    /// </returns>
    Task<List<GameFileIntegrityReport>> GetInstallationIntegrityReportAsync(List<GamePkgVersionEntry> entries, ProgressReporter<ProgressReport>? reporter, CancellationToken token = default);
    
    /// <inheritdoc cref="RepairInstallationAsync(System.Collections.Generic.List{Dodoco.Core.Game.GameFileIntegrityReport}, Dodoco.Core.ProgressReporter{ProgressReport}?, System.Threading.CancellationToken)"/>
    Task<List<GameFileIntegrityReport>> RepairInstallationAsync(List<GameFileIntegrityReport> reports, CancellationToken token = default);
    
    /// <summary>
    /// Attempts to repair all referenced game files from the list provided by downloading
    /// them from the remote server.
    /// </summary>
    /// <returns>
    /// Returns a <see cref="System.Collections.Generic.List{T}(Dodoco.Core.Game.GameFileIntegrityReport)"/>
    /// that references successfully repaired input files.
    /// </returns>
    Task<List<GameFileIntegrityReport>> RepairInstallationAsync(List<GameFileIntegrityReport> reports, ProgressReporter<ProgressReport>? reporter, CancellationToken token = default);

}