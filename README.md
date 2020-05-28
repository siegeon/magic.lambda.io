
# Magic Lambda IO

[![Build status](https://travis-ci.org/polterguy/magic.lambda.io.svg?master)](https://travis-ci.org/polterguy/magic.lambda.io)

This project provides file/folder slots for [Magic](https://github.com/polterguy/magic). More specifically, it provides the following slots.

* __[io.folder.create]__ - Creates a folder on disc for you on your server.
* __[io.folder.exists]__ - Returns true if folder exists, otherwise false.
* __[io.folder.delete]__ - Deletes a folder on disc on your server.
* __[io.file.load]__ - Loads a file from disc on your server.
* __[io.file.save]__ - Saves a file on disc on your server.
* __[io.file.exists]__ - Returns true if file exists, otherwise false.
* __[io.file.delete]__ - Deletes a file on your server.
* __[io.files.copy]__ - Copies a file on your server.
* __[io.files.execute]__ - Executes a Hyperlambda file on your server.
* __[io.files.list]__ - List files in the specified folder on your server.
* __[io.files.move]__ - Moves a file on your server.
* __[io.content.zip-stream]__ - Creates a ZipStream for you, without touching the file system.
* __[io.folder.list]__ - List all folder in the specified folder on your server.
* __[.io.folder.root]__ - Returns the root folder of your system. (private C# slot)

## Fundamentals

All of these slots can _only_ manipulate files and folders inside of your _"files"_ folder in your web server.
This are normally files inside of the folder you have configured in your _"appsettings.json"_ file, with the
key _"magic.io.root-folder"_. This implies that all paths are _relative_ to this path, and no files or folders
from outside of this folder can in any ways be manipulated using these slots.

### io.folder.create

Creates a new folder on disc. Example below.

```
io.folder.create:/misc/foo/
```

### io.folder.exists

Returns true if specified folder exists on disc.

```
io.folder.exists:/misc/foo/
```

### io.folder.delete

Deletes the specified folder on disc.

```
io.folder.create:/misc/foo/
```

### io.file.load

Loads the specified text file from disc.

```
io.file.load:/misc/README.md
```

### io.file.save

Saves the specified content to the specified file on disc, overwriting any previous content if the
file exists from before, creating a new file if no such file already exists.

```
io.file.save:/misc/README2.md
   .:This is new content for file
```

### io.file.exists

Returns true if specified file exists from before.

```
io.file.exists:/misc/README.md
```

### io.file.delete

Deletes the specified file.

```
io.file.load:/misc/README.md
```

### io.files.copy

Copies the specified file to the specified destination folder and file.
Notice, requires the destination folder to exist from before.

```
io.file.load:/misc/README.md
   .:/misc/backup/README-backup.md
```

### io.files.execute

Executes the specified Hyperlambda file.

```
io.files.execute:/misc/some-hyperlambda-file.hl
```

### io.files.list

Lists all files inside of the specified folder.

```
io.files.list:/misc/
```

### io.files.move

```
io.files.move:/misc/README.md
   .:/misc/backup/README-backup.md
```

### io.content.zip-stream

Creates a memory based ZIP stream you can return over the response socket connection.

### io.folder.list

Lists all folders inside of the specified folder.

```
io.folder.list:/misc/
```

### .io.folder.root

Returns the root folder of the system. Cannot be invoked from Hyperlambda, but only from C#. Inended as
a support function for other C# slots.

## License

Although most of Magic's source code is publicly available, Magic is _not_ Open Source or Free Software.
You have to obtain a valid license key to install it in production, and I normally charge a fee for such a
key. You can [obtain a license key here](https://servergardens.com/buy/).
Notice, 5 hours after you put Magic into production, it will stop functioning, unless you have a valid
license for it.

* [Get licensed](https://servergardens.com/buy/)
