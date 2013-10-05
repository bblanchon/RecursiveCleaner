RecursiveCleaner
============

Deletes files or folders according to filters defined in XML files.

It is very handy to:

- clean intermediate files from compilers (the bin and obj folders)
- remove temporary files or backups
- delete old files in your Downloads directory

You just need to add a XML file named RecursiveCleaner.config in the folder you want to clean.

This software require Microsoft .NET Framework 4

Examples
======

Delete old files and folder
-
	<RecursiveCleaner>
	  <Recycle Target="FilesAndFolders">
	    <OlderThan months="3" />
	  </Recycle>
	</RecursiveCleaner>
	
Delete Emacs backups
-
	<RecursiveCleaner>
	  <Recycle Target="Files">
	    <Wildcards>*~</Wildcards>
	  </Recycle>
	</RecursiveCleaner>
    
Delete bin and obj folders
-
	<RecursiveCleaner>
	  <Recycle Target="Folders">
	    <Regex>^bin$|^obj$</Regex>
	  </Recycle>
	</RecursiveCleaner>

Delete old .suo files
-
	<RecursiveCleaner>
	  <Recycle Target="Files">
	    <MatchingAll>
	      <Wildcards>*.suo</Wildcards>
	      <OlderThan months="6" />
	    </MatchingAll>
	  </Recycle>
	</RecursiveCleaner>
    
Move downloaded videos to the right place
-
	<RecursiveCleaner>
	  <Move Target="Files" Destination="C:\Movies">
	    <MatchingAll>
	      <Wildcards>*.avi</Wildcards>
	      <BiggerThan MB="500" />
	    </MatchingAll>
	  </Move>
	</RecursiveCleaner>

Group files by date
-
This will create a folder for each date and move the files to the right folder according to the creation date of the file. (Very useful for a big download directory)

	<RecursiveCleaner>
	  <Move Target="Files" Destination="%source.date%" ApplyToSubFolders="false">
	    <OlderThan weeks="1" />
	  </Move>
	</RecursiveCleaner>

Rename files according to a regex
-
	<RecursiveCleaner>
	  <Rename Name="New name %1%.avi" Target="Files" ApplyToSubFolders="false">
	    <Regex Pattern="^Oldname(.+)\.avi$"/>
	  </Rename>
	</RecursiveCleaner>