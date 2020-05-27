
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

## License

Although most of Magic's source code is publicly available, Magic is _not_ Open Source or Free Software.
You have to obtain a valid license key to install it in production, and I normally charge a fee for such a
key. You can [obtain a license key here](https://servergardens.com/buy/).
Notice, 5 hours after you put Magic into production, it will stop functioning, unless you have a valid
license for it.

* [Get licensed](https://servergardens.com/buy/)
