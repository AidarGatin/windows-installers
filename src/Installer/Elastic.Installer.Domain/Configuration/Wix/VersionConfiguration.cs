﻿using Semver;
using static Elastic.Installer.Domain.Configuration.Wix.InstallationDirection;
using static Elastic.Installer.Domain.Configuration.Wix.VersionChange;

namespace Elastic.Installer.Domain.Configuration.Wix
{
	public class VersionConfiguration
	{
		/// <summary> The version we are upgrading from</summary>
		public SemVersion UpgradeFromVersion { get; }
		
		/// <summary> The version that is currently being installed</summary>
		public SemVersion CurrentVersion { get; }

		public VersionChange VersionChange { get; } 
		public InstallationDirection InstallationDirection { get; }

		public bool ExistingVersionInstalled => VersionChange != New;
		public bool AlreadyInstalled { get; }
		public bool SameVersionAlreadyInstalled => VersionChange == Same;
		public bool HigherVersionAlreadyInstalled => InstallationDirection == Down; 

		public VersionConfiguration(IWixStateProvider wixStateProvider, bool alreadyInstalled)
		{
			this.AlreadyInstalled = alreadyInstalled;
			var c = this.CurrentVersion = wixStateProvider.CurrentVersion;
			var e = this.UpgradeFromVersion = wixStateProvider.UpgradeFromVersion;

			var v = New;
			var d = None;

			if (e != null)
			{
				if (c == e)
				{
					v = Same;
					d = None;
				}
				else if (c > e) d = Up;
				else if (c < e) d = Down;

				if (c.Major != e.Major) v = Major;
				else if (c.Minor != e.Minor) v = Minor;
				else if (c.Patch != e.Patch) v = Patch;
				else if (c.Prerelease != e.Prerelease) v = Prerelease;
			}
			this.VersionChange = v;
			this.InstallationDirection = d;
		}
	}
}