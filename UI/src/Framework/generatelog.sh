#!/bin/bash
# Generates a day to day changelog
SINCETHISYEAR="2022-1-1"
SINCEWEEKAGO="1 week ago"
NEXT=$(date +%F)
git log --no-merges --format="%cd" --since=$SINCETHISYEAR --date=short | sort -u -r | while read DATE ; do
    echo
    echo [$DATE]
    GIT_PAGER=cat git log --no-merges --format=" - [PR](https://dev.azure.com/amcsgroup/Platform/_git/PlatformUIFramework/pullrequest/)	%s" --since=$DATE --until=$NEXT
    NEXT=$DATE
done

