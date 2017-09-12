# timelapse.net
Create timelapse and control a Canon EOS 350D from a raspberry pi with asp.net core


# setup raspberry pi
https://iotbytes.wordpress.com/setting-up-docker-on-raspberry-pi-and-running-hello-world-container/

    # Install Raspbian
    
    # Update Packages
    sudo apt-get update && sudo apt-get upgrade
    
    # Install Docker
    curl -sSL https://get.docker.com | sh
    
    # Add permission to Pi User to run Docker Commands
    sudo usermod -aG docker pi
    
    #Verify Installation
    docker --version


# dotnet-gphoto2 docker image
https://stackoverflow.com/questions/24225647/docker-any-way-to-give-access-to-host-usb-or-serial-device

    docker run -it --rm --privileged -v /dev/bus/usb:/dev/bus/usb lakerfield/dotnet-gphoto2 bash

    #test camera connection
    gphoto2 --summary
    
    #capture photo and download from camera
    gphoto2 --capture-image-and-download


# timelapse-net-pi

    # docker host (raspberry pi)
    sudo mkdir /data
    docker run --rm --privileged -v /dev/bus/usb:/dev/bus/usb -v /data:/data lakerfield/timelapse-net-pi Bertus


# on raspberry pi

    docker pull lakerfield/timelapse-aspnet-pi:latest
    docker run --rm -p 80:80 lakerfield/timelapse-aspnet-pi:latest
    docker run --rm --privileged -v /dev/bus/usb:/dev/bus/usb -v /data:/data -p 80:80 lakerfield/timelapse-aspnet-pi:latest
    docker run --name=picam --rm --privileged -v /dev/bus/usb:/dev/bus/usb -v /data:/data -p 80:80 lakerfield/timelapse-aspnet-pi:latest
    docker run --name=picam -d --rm --privileged -v /dev/bus/usb:/dev/bus/usb -v /data:/data -p 80:80 lakerfield/timelapse-aspnet-pi:latest
    docker run --name=picam -d --restart unless-stopped --rm --privileged -v /dev/bus/usb:/dev/bus/usb -v /data:/data -p 80:80 lakerfield/timelapse-aspnet-pi:latest



# links
Build on Windows or macOS and run the sample with Docker on Linux + ARM32 (Raspberry Pi)
https://github.com/dotnet/dotnet-docker-samples/tree/master/dotnetapp-selfcontained#build-on-windows-or-macos-and-run-the-sample-with-docker-on-linux--arm32-raspberry-pi
