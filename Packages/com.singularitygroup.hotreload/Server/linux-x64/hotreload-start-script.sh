#!/bin/sh

set -e

CLIARGUMENTS_FILE=""
EXECUTABLESOURCEDIR=""
EXECUTABLETARGETDIR=""
TITLE=""
METHODPATCHDIR=""
PIDFILE=""

while [ "$1" != "" ]; do
    case $1 in
        -c | --cli-arguments-file )
            shift
            CLIARGUMENTS_FILE="$1"
            ;;
        --executables-source-dir )
            shift
            EXECUTABLESOURCEDIR="$1"
            ;;
        --executable-taget-dir )
            shift
            EXECUTABLETARGETDIR="$1"
            ;;
        --title )
            shift
            TITLE="$1"
            ;;
        --create-no-window )
            shift
            CREATENOWINDOW="$1"
            ;;
        -p | --pidfile )
            shift
            PIDFILE="$1"
            ;;
        -m | --method-patch-dir )
            shift
            METHODPATCHDIR="$1"
            ;;
    esac
    shift
done

if [ -z "/tmp/HotReloadTemp" ] || [ -z "$CLIARGUMENTS_FILE" ] || [ -z "$EXECUTABLESOURCEDIR" ] || [ -z "$EXECUTABLETARGETDIR" ] || [ -z "$TITLE" ] || [ -z "$PIDFILE" ] || [ -z "$METHODPATCHDIR" ] || [ -z "$CREATENOWINDOW" ]; then
    echo "Missing arguments"
    exit 1
fi

CLIARGUMENTS=$(cat "$CLIARGUMENTS_FILE")
rm "$CLIARGUMENTS_FILE"

# Needs be removed if you have multiple unities
pgrep CodePatcherCLI | xargs -I {} kill {}

rm -rf "$METHODPATCHDIR"
SERVER="$EXECUTABLETARGETDIR/CodePatcherCLI"

TERMINALRUNSCRIPT="$EXECUTABLESOURCEDIR/terminal-run.sh"
sed -i 's/\r//g' "$TERMINALRUNSCRIPT"

chmod +x "$TERMINALRUNSCRIPT"
chmod +x "$SERVER"

HAVETERMINAL=""
"$TERMINALRUNSCRIPT" && HAVETERMINAL="yes"

INTERNALSCRIPT="$EXECUTABLETARGETDIR/hotreload-internal-start"

# see doc/linux-system-freeze.org why I put the nice

cat << EOF > "$INTERNALSCRIPT"
#!/bin/sh
echo \$\$ > "$PIDFILE"
nice -n 5 "$SERVER" $CLIARGUMENTS || read
EOF

chmod +x "$INTERNALSCRIPT"

if [[ -n "$HAVETERMINAL" && "$CREATENOWINDOW" != "True" ]]; then
    "$TERMINALRUNSCRIPT" "$TITLE" "$INTERNALSCRIPT"
else
    printf "Don't have a terminal to run, printing to unity console instead. Consider hacking:\n%s\n" "$TERMINALRUNSCRIPT"
    exec "$INTERNALSCRIPT"
fi
