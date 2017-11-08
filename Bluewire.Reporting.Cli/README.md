# Bluewire.Reporting.Cli

Command-line tool for deploying reports to SSRS

## General Concepts

Three types of SSRS object are understood:

* Report
* Dataset
* Data Source

Each object has a path written as an absolute URI path segment, eg. `/Reports/Letters/Weekly Production`. Whitespace is permitted in path segments. Symbols are permitted, but not recommended.

Objects may be read from a directory hierarchy, a zip archive, a single file, or an SSRS instance.

* Objects read from a directory or zip archive are given paths corresponding to their file name and relative path within that directory or archive. This may be modified with the `--base` switch.
* A object read from a file has the path `/<file-name>`. This may be modified with the `--base` switch.
* Objects read from SSRS are assigned paths by SSRS, and are not affected by the `--base` switch.

* If the specified directory or zip archive contains a `report-manifest.xml` file as an immediate child (not in a subdirectory), this may be used for filtering objects using the `--site` switch.

Globs may be used for filtering on paths. The leading `/` is assumed and may be omitted. The glob is matched against the entire path, not just a prefix. Supported wildcards are:
* `?` matches exactly one character (not `/`).
* `*` matches any number of characters (not `/`).
* `**` matches any number of characters, including `/`.

## Usage

### Modes

#### `Bluewire.Reporting.Cli inspect [--base=<base-path>] [--site=<site>] [--include=<path-glob>] [--exclude=<path-glob>] [--type=report,dataset,datasource] <ssrs-uri|file|directory>...`

Read matched objects from the specified sources and describe them on STDOUT.


#### `Bluewire.Reporting.Cli create-datasource [--type=SQLServer] --connection-string=<connection-string> [--store=[DOMAIN\]username:password] [--prompt] [--integrated] [--overwrite] <ssrs-uri> <object-path>`

Create a new SQL Server data source object at `<object-path>` on the specified SSRS instance, using the provided properties.

* `--store` configures stored credentials for the data source.
  * By default, these will be treated as SQL Server credentials.
  * If `--integrated` is also specified, they will be treated as Windows credentials.
* `--integrated`, if specified without `--store`, configures the data source to use Windows Integrated authentication, ie. the credentials of the user or process requesting the report.
* `--prompt` configures the data source to ask for credentials.
* Otherwise, the data source will be configured not to use credentials.
* Any combination of authentication rules not explicitly described here is treated as an error.

* If the data source exists and `--overwrite` is not specified, an error will be returned.


#### `Bluewire.Reporting.Cli import [--base=<base-path>] [--site=<site>] [--include=<path-glob>] [--exclude=<path-glob>] [--type=report,dataset,datasource] [--overwrite] [--backup=<backup-directory>] [--rewrite=<rule>...] <ssrs-uri> <files|directories...>`

Import matched objects from one or more sources into the specified SSRS instance.

* If `--overwrite` is not specified, existing objects will be silently skipped.
* If `--overwrite` is specified, existing objects will be silently replaced.
  * `--backup` may be specified to export existing objects before they are replaced.
* `--rewrite` may be used to modify objects as they are imported. See the *Rewrites* section below.


### Common parameters

The following parameters are supported by multiple modes:
* `--base=<base-path`: Prepend the specified path to objects read from the filesystem.
* `--include=<path-glob>`: If specified, only include objects matching the specified path glob.
* `--exclude=<path-glob>`: If specified, omit objects matching the specified path glob. This takes precedence over `--include` if both are specified.
* `--type=report,dataset,datasource`: Include only the specified types of object.
* `--site=<site>`: If at least one object source contains a report manifest, includes only the objects related to that site. If no source contains a manifest, it is an error to specify this switch.

The following parameters are supported by all modes:
* `--verbose` or `-v`: Increase verbosity of informational console output (STDERR).
* `--quiet` or `-q`: Decrease verbosity of informational console output (STDERR).
* `--pause` or `-p`: Wait for a key to be pressed before exiting.

### Exit Codes

* 0 - Success
* 1 - Invalid arguments
* All others - other error

### Rewrites

Rewrite `<rule>`s take the following forms:
* `DataSet.DataSourceReference:{<glob>}=<reference>`: Replace all imported data sets' data source references with the specified data source, if their existing reference matches the glob pattern.
   If the reference does not have a trailing `/`, it is treated as being relative to the data set's container.
* `DataSet.DataSourceReference:<reference>`: Shorthand for `DataSet.DataSourceReference:{**}=<reference>`, ie. match all existing references.


## Report Manifests

A report manifest is an XML file which specifies named filters for the objects in a directory or archive. It consists of zero or more path prefixes, each associated with zero or more site names. The manifest *must* be called `report-manifest.xml`.

    <Reports>
	  <Report Path="Letters/">
		<Include Site="Site A" />
		<Include Site="Site C" />
	  </Report>
	  <Report Path="Notes/">
		<Include Site="Site B" />
	  </Report>
	  <Report Path="/Admin">
		<Include Site="Site C" />
	  </Report>
	</Reports>

* When `--site='Site A'` is specified, only objects in `/Letters/` will be enumerated.
* When `--site='Site B'` is specified, only objects in `/Notes/` will be enumerated.
* When `--site='Site C'` is specified, objects in `/Letters/` will be enumerated along with any matching '/Admin**', in eg. `/Administrative/`, `/administration/`,`/Admin Reports/`
* When `--site='Site D'` is specified, no objects will be enumerated.

* The manifest paths are treated as relative to the directory or archive in which the manifest resides and are not broken by `--base`.
* Paths and site names are case-insensitive.

## General Architecture

Objects are read from one or more ISsrsObjectSource instances, bundled in an AggregatedObjectSource.
ISsrsObjectSource supports lazy enumeration of objects based on a filter. Filesystem implementations also take a BasePath for generating object paths.

Command-line parsing works as follows:
1. The first argument is assumed to be the mode. This is matched case-sensitively against known modes and an IJobFactory is created.
1. The ConsoleSession's argument parser is configured by the IJobFactory. Arguments are used to initialise properties of the IJobFactory.
1. After parsing succeeds, an IJob is created. Semantic validation is performed as part of this stage.
1. The IJob is run against STDOUT.

### Data Types and Helpers

SsrsObjectPath encapsulates object paths. Each path is logically comprised of one or more segments, each preceded by a `/`. The sole exception is the root path, which is simply `/`.

PathFilter generates IPathFilter objects which test a given path against a pattern. These also expose a 'static prefix', a list of leading non-wildcard segments which can be used to optimise querying against a hierarchy (currently only used against SSRS).

