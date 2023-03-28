# Installing Plugin to Core

* The installPlugin.bat batch file extracts zipped RegisterPlugin to temp directory, calls the exe passing in required parameters, then deletes the temp folder.
* CopyTool will update the values in batch file to correct new values.
* Core must be running locally at default port 26519 before running the batch file.
* To add dependencies for plugin add them in the metadata.json file, and they will get picked up