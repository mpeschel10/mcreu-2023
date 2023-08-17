call rsync_to_mpeschel10 Builds/Quest/mcreu-2023.apk /var/www/mcreu/quest-build
call rsync_to_mpeschel10 Builds/WebGL/ /var/www/mcreu/webgl

cd Builds
7z a -tzip PC-Build.zip .\PC-Build\*
cd ..

call rsync_to_mpeschel10 Builds/PC-Build.zip /var/www/mcreu/pc-build

