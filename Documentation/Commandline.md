# Command Line usage

For the tool to work in the work folder there must be a bookgen.json config file.
This config file can be created with the following command:

```
BookGen Build -a Init
```

To get information regarding the configuration file bookgen.json file run:
```
Bookgen ConfigHelp
```

## GUI Usage
```
    BookGen Gui {-v} {-d [directory]}
    BookGen Gui {--verbose} {--dir [directory]}

    Arguments:
    -d, --dir:
        Optional argument. Specifies work directory. If not specified, then
        the current directory will be used as working directory.
    -v, --verbose: 
        Optional argument, turns on detailed logging. Usefull for locating issues
```

## Update Usage
```
    BookGen Update {-p} {-s}
    BookGen Update {--prerelease} {--searchonly}

    Arguments:
      -s, --searchonly:
        Optional argument. Search and display latest release,
        but don't actually do an update
     -p, --prerelease:
        Include pre relase versions.
```

## Build Usage
```
    BookGen Build -a [action] {-v} {-d [directory]} {-n}
    BookGen Build --action [action] {--verbose} {--dir [directory]} {--nowait}

  Arguments:
    -a, --action: 
        Specifies the build action. See below.
    -d, --dir:
        Optional argument. Specifies work directory. If not specified, then
        the current directory will be used as working directory.
    -v, --verbose: 
        Optional argument, turns on detailed logging. Usefull for locating issues
    -n, --nowait:
        Optional argument, when specified & the program is fisihed,
        then it immediately exits, without key press.
```
### Build Actions:
* ```BuildEpub``` - Build Ebup 3.2
* ```BuildPrint``` - Build Printable HTML document
* ```BuildWeb``` - Build Releaseable Static Website
* ```BuildWordpress``` - Build Wordpress Export XML file
* ```Clean``` - Clean output folders
* ```Initialize``` - Initialize dir as BookGen project
* ```Test``` - Build Test Static Website & Run test server
* ```ValidateConfig``` - Validates the bookgen.json configuration file