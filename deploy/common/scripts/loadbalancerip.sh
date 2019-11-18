#!/bin/bash

i=0
ip=""
while [ $i -le 300 ]
do
    ip=$(kubectl get svc -o custom-columns=:.status.loadBalancer.ingress[0].ip | grep -E '[0-9]+\.[0-9]+\.[0-9]+\.[0-9]+')
    if [ "$ip" == "" ] || [ $ip == "null" ]
    then
        sleep 1;
    else
        break;
    fi
    i=$(($i + 1))
done

echo $ip