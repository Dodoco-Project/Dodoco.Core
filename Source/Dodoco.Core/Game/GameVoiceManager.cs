namespace Dodoco.Core.Game;

using Dodoco.Core.Extension;
using Dodoco.Core.Network.HTTP;
using Dodoco.Core.Protocol.Company.Launcher.Resource;
using Dodoco.Core.Util.Hash;
using Dodoco.Core.Util.Log;

using UrlCombineLib;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

/// <summary>
/// Class <c>GameVoiceManager</c> contains methods to manage game's voices packages.
/// </summary>
public partial class GameVoiceManager: IGameVoiceManager {

    protected readonly IGame Game;

    protected GameVoiceManagerState _State = GameVoiceManagerState.IDLE;
    public GameVoiceManagerState State {
        get => _State;
        protected set {
            Logger.GetInstance().Debug($"Updating {nameof(GameVoiceManagerState)} from {_State} to {value}");
            _State = value;
        }
    }

    [GeneratedRegex("(\\/)(Audio_)[a-zA-Z|(|)]+(_)([\\d]+.)+(zip)")]
    protected static partial Regex VoicePackageFilenamePattern();
    
    public GameVoiceManager(IGame game) => Game = game;

    /// <inheritdoc />
    public virtual IEnumerable<GameLanguage> GetInstalledVoices() {

        Logger.GetInstance().Log($"Checking installed game voice packages...");

        List<GameLanguage> installedVoices = new List<GameLanguage>();

        foreach (GameLanguage language in GameLanguage.All) {

            if (Directory.Exists(Path.Join(Game.Settings.InstallationDirectory, Game.GetDataDirectoryName(), "/StreamingAssets/AudioAssets/", language.Name))) {

                Logger.GetInstance().Log($"The voice package \"{language.Name}\" ({language.Code}) is installed");
                installedVoices.Add(language);

            }

        }

        Logger.GetInstance().Log($"Successfully checked installed game voice packages");

        return installedVoices;

    }

    protected virtual async Task<ResourceVoicePack> GetLatestVoicePackageAsync(GameLanguage language) {

        Logger.GetInstance().Log($"Searching the voice package for the language \"{language.Name}\"...");

        ResourceResponse resource = await Game.GetApiFactory().FetchLauncherResource();
        resource.EnsureSuccessStatusCode();

        Predicate<ResourceVoicePack> desiredResourceVoicePack = (voicePack) => voicePack.language.ToUpper() == language.Code.ToUpper();
        
        if (!resource.data.game.latest.voice_packs.Exists(desiredResourceVoicePack)) {

            throw new GameException($"Unable to find the voice package for the language \"{language.Name}\"");

        }

        Logger.GetInstance().Log($"Successfully found the voice package for the language \"{language.Name}\"");

        return resource.data.game.latest.voice_packs.Find(desiredResourceVoicePack);

    }

    protected virtual string GetVoicePackageFilename(ResourceVoicePack voicePack) {

        if (!VoicePackageFilenamePattern().IsMatch(voicePack.path)) {

            throw new GameException($"Unable to get the voice package filename with the given regex pattern \"{VoicePackageFilenamePattern()}\" in the string \"{voicePack.path}\"");

        }

        return VoicePackageFilenamePattern().Match(voicePack.path).ToString();

    }

    protected virtual bool IsVoicePackageDownloaded(ResourceVoicePack voicePack, GameLanguage language) {

        GameVoiceManagerState previousState = State;

        try {

            string filepath = Path.Join(Game.Settings.InstallationDirectory, this.GetVoicePackageFilename(voicePack));

            if (File.Exists(filepath)) {

                this.State = GameVoiceManagerState.RECOVERING_DOWNLOADED_VOICE_PACKAGE;
                Logger.GetInstance().Log($"Recovering the already downloaded voice package for the language \"{language.Name}\"...");

                string checksum = new Hash(MD5.Create()).ComputeHash(filepath);
                
                if (checksum.ToUpper() == voicePack.md5.ToUpper()) {

                    Logger.GetInstance().Log($"The voice package for the language \"{language.Name}\" is already downloaded and its MD5 checksum ({checksum.ToUpper()}) matches the remote one");
                    return true;

                } else {

                    Logger.GetInstance().Warning($"The voice package for the language \"{language.Name}\" is downloaded but its MD5 checksum ({checksum.ToUpper()}) doesn't matches the remote one ({voicePack.md5.ToUpper()})");

                }

            } else {

                Logger.GetInstance().Warning($"The voice package for the language \"{language.Name}\" is not downloaded");

            }

        } catch (CoreException) {

            throw;

        } finally {

            this.State = previousState;

        }

        return false;

    }

    protected virtual async Task DownloadVoicePackageAsync(ResourceVoicePack voicePack, string packagePath, GameLanguage language, ProgressReporter<ProgressReport>? reporter, CancellationToken token = default) {

        GameVoiceManagerState previousState = State;

        try {

            this.State = GameVoiceManagerState.DOWNLOADING_VOICE_PACKAGE;
            
            Logger.GetInstance().Log($"Downloading the voice package for the language \"{language.Name}\"...");
            
            await Client.GetInstance().DownloadFileAsync(new Uri(voicePack.path), packagePath, reporter, token);
            
            Logger.GetInstance().Log($"Successfully downloaded the voice package for the language \"{language.Name}\"");

        } catch (CoreException) {

            throw;

        } finally {

            this.State = previousState;

        }

    }

    protected virtual void UnzipVoicePackage(string packagePath, GameLanguage language, ProgressReporter<ProgressReport>? reporter, CancellationToken token = default) {

        GameVoiceManagerState previousState = State;

        try {

            if (!File.Exists(packagePath)) {

                throw new GameException($"The voice package file for the language \"{language.Name}\" is missing");

            }

            this.State = GameVoiceManagerState.UNZIPPING_VOICE_PACKAGE;
            Logger.GetInstance().Log($"Unzipping the voice package for the language \"{language.Name}\"...");

            using (FileStream zipFileStream = File.OpenRead(packagePath)) {

                using(ZipArchive zipArchive = new ZipArchive(zipFileStream, ZipArchiveMode.Read)) {

                    // overwrite files = true
                    zipArchive.ExtractToDirectory(Path.Join(this.Game.Settings.InstallationDirectory, this.Game.GetDataDirectoryName(), "/StreamingAssets/AudioAssets"), true, reporter);

                }

            }

            Logger.GetInstance().Log($"Successfully unzipped the voice package for the language \"{language.Name}\"");

        } catch (CoreException) {

            throw;

        } finally {

            this.State = previousState;

        }

    }

    /// <inheritdoc />
    public virtual async Task InstallVoicePackageAsync(GameLanguage language, bool forceReinstall, ProgressReporter<ProgressReport>? reporter, CancellationToken token = default) {

        if (this.State != GameVoiceManagerState.IDLE) {

            throw new GameException($"The voice package manager is busy");

        }
        
        if (!forceReinstall && GetInstalledVoices().Contains(language)) {

            throw new GameException($"The voice package for the language \"{language.Name}\" is already installed");
        
        }

        ResourceVoicePack voicePack = await GetLatestVoicePackageAsync(language);
        string packagePath = Path.Join(Game.Settings.InstallationDirectory, this.GetVoicePackageFilename(voicePack));

        if (!this.IsVoicePackageDownloaded(voicePack, language)) {

            await this.DownloadVoicePackageAsync(voicePack, packagePath, language, reporter, token);

        }

        this.UnzipVoicePackage(packagePath, language, reporter, token);

    }

    protected virtual string GetVoicePackageUpdateFilenamePattern(GameLanguage language, Version currentVersion, Version targetVersion) {

        return @$"({language.Code.ToLower()}_{currentVersion.ToString().Replace(".", @"\.")}_{targetVersion.ToString().Replace(".", @"\.")}_hdiff_)[\w*]+(\.zip)";

    }

    public virtual async Task<ResourceVoicePack> GetVoicePackageUpdateAsync(ResourceDiff resourceDiff, GameLanguage language) {

        Version currentVersion = await this.Game.GetGameVersionAsync();
        Version remoteVersion = Version.Parse(resourceDiff.version);
        string voicePackageFilenamePattern = this.GetVoicePackageUpdateFilenamePattern(language, currentVersion, remoteVersion);
        Predicate<ResourceVoicePack> desiredVoicePack = (package) => Regex.IsMatch(package.path, voicePackageFilenamePattern);

        if (resourceDiff.voice_packs.Exists(desiredVoicePack)) {

            return resourceDiff.voice_packs.Find(desiredVoicePack);

        } else {

            throw new GameException($"Can't find a diff object whose name matchs the string pattern \"{voicePackageFilenamePattern}\"");

        }

    }

    public virtual async Task<List<GamePkgVersionEntry>> GetVoicePackagePkgVersionAsync(GameLanguage language) {

        Uri pkgVersionRemoteUrl = new Uri(UrlCombine.Combine((await this.Game.GetResourceAsync()).data.game.latest.decompressed_path.ToString(), language.PkgVersionFilename));
        HttpResponseMessage response = await Client.GetInstance().FetchAsync(pkgVersionRemoteUrl);

        if (response.IsSuccessStatusCode) {

            return PkgVersionParser.Parse(await response.Content.ReadAsStringAsync());

        } else {

            throw new GameException($"Failed to fetch the {language.PkgVersionFilename} file from remote servers (received HTTP status code {response.StatusCode})");

        }

    }

    protected virtual async Task ApplyVoicePackageUpdatePatches(string packagePath, GameLanguage language, ProgressReporter<ProgressReport>? reporter, CancellationToken token = default) {

        GameVoiceManagerState previousState = this.State;

        try {

            using (FileStream file = File.OpenRead(packagePath))
            using (ZipArchive zipArchive = new ZipArchive(file, ZipArchiveMode.Read)) {

                ZipArchiveEntry? hdiffListArchive = zipArchive.GetEntry("hdifffiles.txt");

                if (hdiffListArchive != null) {

                    this.State = GameVoiceManagerState.APPLYING_VOICE_PACKAGE_UPDATE;
                    Logger.GetInstance().Log($"Applying the patches from the \"{language.Name}\" voice package...");

                    using (Stream hdiffListStream = hdiffListArchive.Open()) {

                        GameHDiffPatcher patcher = new GameHDiffPatcher();
                        int appliedPatchesCount = 0;
                        List<GameHDiffFilesEntry> patchesList = HDiffListParser.ParseAll(hdiffListStream);

                        await Parallel.ForEachAsync(patchesList, async (entry, token) => {

                            string oldFilePath = Path.Join(this.Game.Settings.InstallationDirectory, entry.remoteName);
                            string backupFilePath = oldFilePath + ".bak";
                            string patchFilePath = oldFilePath + ".hdiff";

                            try {

                                Logger.GetInstance().Log($"Patching the file \"{oldFilePath}\"...");

                                // Creates a backup of the file from current game's version
                                File.Copy(oldFilePath, backupFilePath);

                                // Patches the old file (it becomes the newer/updated file)
                                await patcher.Patch(patchFilePath, backupFilePath, oldFilePath);

                                // Removes the backup file and the patch file
                                File.Delete(patchFilePath);

                                Logger.GetInstance().Log($"Successfully patched the file \"{oldFilePath}\"");

                                appliedPatchesCount++;
                                
                                reporter?.Report(new ProgressReport {

                                    Done = appliedPatchesCount,
                                    Total = patchesList.Count,
                                    Message = oldFilePath

                                });

                            } catch (Exception e) {

                                Logger.GetInstance().Error($"Error while patching the file \"{oldFilePath}\"", e);
                                
                            } finally {

                                if (File.Exists(backupFilePath)) {

                                    File.Copy(backupFilePath, oldFilePath);
                                    File.Delete(backupFilePath);

                                }

                            }

                        });

                    }

                    Logger.GetInstance().Log($"Successfully applied the patches from the \"{language.Name}\" voice package");

                } else {

                    Logger.GetInstance().Warning($"This voice update package doesn't contain patches");

                }

            }

        } catch (CoreException) {

            throw;

        } finally {

            this.State = previousState;

        }

    }

    protected virtual void RemoveVoicePackageDeprecatedFiles(string packagePath, GameLanguage language, ProgressReporter<ProgressReport>? reporter, CancellationToken token = default) {

        GameVoiceManagerState previousState = this.State;

        try {

            using (FileStream file = File.OpenRead(packagePath))
            using (ZipArchive zipArchive = new ZipArchive(file, ZipArchiveMode.Read)) {

                ZipArchiveEntry? deprecatedFilesListArchive = zipArchive.GetEntry("deletefiles.txt");

                if (deprecatedFilesListArchive != null) {

                    this.State = GameVoiceManagerState.REMOVING_DEPRECATED_FILES;
                    Logger.GetInstance().Log($"Removing deprecated files from the \"{language.Name}\" voice package...");

                    using (Stream deprecatedFilesListStream = deprecatedFilesListArchive.Open()) {

                        GameHDiffPatcher patcher = new GameHDiffPatcher();
                        int removedFilesCount = 0;
                        List<string> deprecatedFilesList = DeprecatedFilesListParser.ParseAll(deprecatedFilesListStream);

                        Parallel.ForEach(deprecatedFilesList, filePath => {

                            string fullPath = Path.Join(this.Game.Settings.InstallationDirectory, filePath);

                            try {

                                Logger.GetInstance().Log($"Removing the file \"{fullPath}\"...");

                                File.Delete(fullPath);
                                
                                Logger.GetInstance().Log($"Successfully removed the file \"{fullPath}\"");

                                removedFilesCount++;

                                reporter?.Report(new ProgressReport {

                                    Done = removedFilesCount,
                                    Total = deprecatedFilesList.Count,
                                    Message = fullPath

                                });

                            } catch (Exception e) {

                                Logger.GetInstance().Error($"Failed to remove the file \"{fullPath}\"", e);

                            }

                        });

                    }

                    Logger.GetInstance().Log($"Successfully removed the deprecated files from the \"{language.Name}\" voice package");

                } else {

                    Logger.GetInstance().Warning($"This voice update package doesn't contain deprecated files");

                }

            }

        } catch (CoreException) {

            throw;

        } finally {

            this.State = previousState;

        }

    }

    public virtual async Task UpdateVoicePackageAsync(IGameUpdateManager updateManager, IGameIntegrityManager integrityManager, GameLanguage language, ProgressReporter<ProgressReport>? reporter, CancellationToken token = default) {

        if (this.State != GameVoiceManagerState.IDLE) {

            throw new GameException($"The voice package manager is busy");

        }
        
        GameVoiceManagerState previousState = State;

        try {

            ResourceGame gameResource = await updateManager.GetGameUpdateAsync() ?? throw new GameException("Game update is not available");
            ResourceDiff resourceDiff = await updateManager.GetGameUpdatePackageDiffAsync(gameResource);
            Version remoteVersion = Version.Parse(resourceDiff.version);

            ResourceVoicePack voicePack = await this.GetVoicePackageUpdateAsync(resourceDiff, language);
            string packagePath = Path.Join(Game.Settings.InstallationDirectory, voicePack.name);

            await Task.WhenAll(
                integrityManager.RepairInstallationAsync(
                    await integrityManager.GetInstallationIntegrityReportAsync(
                        await this.GetVoicePackagePkgVersionAsync(language),
                        reporter,
                        token
                    ),
                    reporter,
                    token
                ),
                new Task(async () => {

                    if (!this.IsVoicePackageDownloaded(voicePack, language)) {

                        await this.DownloadVoicePackageAsync(voicePack, packagePath, language, reporter, token);

                    }

                })
            );
            
            this.UnzipVoicePackage(packagePath, language, reporter, token);
            await this.ApplyVoicePackageUpdatePatches(packagePath, language, reporter, token);
            this.RemoveVoicePackageDeprecatedFiles(packagePath, language, reporter, token);
            
            Logger.GetInstance().Log($"Sucessfully updated the voice package for the language \"{language.Name}\" to version {remoteVersion}");

        } catch (CoreException) {

            throw;

        } finally {

            this.State = previousState;

        }

    }

    public virtual void RemoveVoicePackage(GameLanguage language) {

        if (!this.GetInstalledVoices().Contains(language)) {

            throw new GameException($"The voice package for the language \"{language.Name}\" is not installed");

        }

        Logger.GetInstance().Log($"Removing the voice package for the language \"{language.Name}\"...");

        string voicePackageDirectory = Path.Join(Game.Settings.InstallationDirectory, Game.GetDataDirectoryName(), "/StreamingAssets/AudioAssets/", language.Name);
        Directory.Delete(voicePackageDirectory, true);
        
        Logger.GetInstance().Log($"Successfully removed the voice package for the language \"{language.Name}\"");

    }

}