#!/bin/bash

# Run a terminal with title and command

# User can already hack this file with their own terminal etc.

    # I didn't check the other ones
    # TODO: User can provide a "terminal-program" in the settings, say you can get inspired by Packages/CodePatcher/Server/linux-x64/terminal-run.sh
    # the script is run with 2 args, a title and a command script to run.

# If called with 0 args, signal the capability to start a terminal.
# If you add your own terminal, make sure to also return 0 when called with 0 args.

TITLE="$1"
COMMAND="$2"

if [ -z "$1" ]; then
    [ -x "$(command -v gnome-terminal)" ] && exit 0
    [ -x "$(command -v xterm)" ] && exit 0
    [ -x "$(command -v konsole)" ] && exit 0
    [ -x "$(command -v terminator)" ] && exit 0
    [ -x "$(command -v urxvt)" ] && exit 0
    [ -x "$(command -v Alacritty)" ] && exit 0
    exit 1
fi

if [ -x "$(command -v gnome-terminal)" ]; then
    gnome-terminal --title="$TITLE" -- "$SHELL" -c "$COMMAND"
elif [ -x "$(command -v xterm)" ]; then
    xterm -title "$TITLE" -e "$SHELL -c '$COMMAND'"
elif [ -x "$(command -v konsole)" ]; then
    konsole --title "$TITLE" --noclose -e "$SHELL -c '$COMMAND'"
elif [ -x "$(command -v terminator)" ]; then
    terminator --title="$TITLE" --command="$SHELL -c '$COMMAND'"
elif [ -x "$(command -v urxvt)" ]; then
    urxvt -title "$TITLE" -e "$SHELL" -c "clear && $COMMAND"
elif [ -x "$(command -v Alacritty)" ]; then
    alacritty -t "$TITLE" -e "$SHELL -c '$COMMAND'"
fi
