cd Builds
7z a -tzip PC-Build.zip .\PC-Build\*
cd ..

call rsync_to_mpeschel10 Builds/PC-Build.zip /var/www/mcreu/pc-build
call rsync_to_mpeschel10 Builds/WebGL/ /var/www/mcreu/webgl
