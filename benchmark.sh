#!/bin/bash

declare -i seconds=0
declare -i limit=3
declare simulator=$1
declare filePath=SystemData.log

true > $filePath
if [[ "$simulator" == "carla" ]] || [[ "$simulator" == "svl" ]] ; then
    while [ "$seconds" -le "$limit" ]; do 
        for i in {1..20}; do echo -n '-' >> $filePath; done
        echo -ne "$(tput setaf 7)$(date +"%H:%M:%S"): \n"
        echo -ne "\nCPU: " >> $filePath
        ps -eo pid,pcpu,pmem,args --sort=-pmem |head | grep svl | cut -b 7-12 >> $filePath
        echo -ne "MEM: " >> $filePath
        ps -eo pid,pcpu,pmem,args --sort=-pmem |head | grep svl | cut -b 13-15 >> $filePath

        echo -ne "GPU-MEM: " >> $filePath 
        declare memory=nvidia-smi | grep $simulator | grep -o '....MiB'
        if [[ "$memory" == "" ]] ; then
            nvidia-smi | grep "simulator" | grep -o '....MiB' >> $filePath
            nvidia-smi | grep "Carla" | grep -o '....MiB' >> $filePath
        else
            echo memory >> $filePath
        fi
        echo -ne "GPU-USG: " >> $filePath
        nvidia-smi | awk 'NR==10' | cut -b 62-64 >> $filePath

        seconds=$(( seconds + 1 ))
        sleep 1; 
    done
else
    echo "$(tput setaf 1)[Error] $(tput setaf 7)- parameter must be svl or carla"
fi
