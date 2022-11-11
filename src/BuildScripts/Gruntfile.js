
module.exports = function(grunt) {

    grunt.option('buildFolder','../build');
    grunt.option('solutionName','MethodFitness');
    grunt.option('projectName','MF.Web');
    grunt.option('srcFolder', '../'+grunt.option('projectName')+'/');
    grunt.option('buildConfig',grunt.option('buildConfig') || 'Debug');
    grunt.option('target',grunt.option('target') || 'QA');
    grunt.option('slnFile','../'+grunt.option('solutionName')+'.sln');
    grunt.option('deployFolder','../../build/MethodFitness_'+grunt.option('target'));
    grunt.option('startTime',new Date());

    grunt.option( 'QA',{
        connection_String:'Server=cannibalcoder.cloudapp.net;Database=MethodFitness_QA;User ID=methodFitness;Password=m3th0d;Connection Timeout=30;',
        host_Name:'http://mfqa.methodfit.net',
        AdminEmail:'methodfit_qa@methodfit.com',
        EmailReportAddress:'',
        environment:grunt.option('target'),
        customErrors:"Off",
        debug:"false",
        version:grunt.file.readJSON('package.json').version
    });

    grunt.option( 'PROD',{
       connection_String:'Server=localhost;Database=MethodFitness_PROD;User ID=methodFitness;Password=m3th0df1t;Connection Timeout=30;',
       host_Name:'http://methodfit.net',
        AdminEmail:'methodfit@methodfit.com',
        environment:grunt.option('target'),
        EmailReportAddress:'',
        customErrors:"RemoteOnly",
        debug:"false",
        version:grunt.file.readJSON('package.json').version
    });

    grunt.initConfig({
        pkg: grunt.file.readJSON('package.json'),
        buildFolder: grunt.option('buildFolder'),
        // tasks
        clean: {
            build: {
                src: [grunt.option('buildFolder')],
                options: { force: true },
            }
        },
        msbuild: {
            src: [grunt.option('slnFile')],

            options: {
                projectConfiguration: grunt.option('buildConfig'),
                platform: 'Any CPU',
                targets: ['Clean', 'Build'],
                version: 4.0,
                verbosity: 'quiet'
            }
        },
        copy: {
            buildArtifacts: {
                expand: true,
                deleteEmptyFolders: true,
                cwd: grunt.option('srcFolder'),
                src: ['**',
                    //'!**/obj/**',
                    '!**/*.cs',
                    '!**/*.vb',
                    '!**/*.csproj',
                    '!**/*.csproj.*',
                    '!Web.config',
                    '!Web.config.hbs',
                    '!*.hbs'

                ],
                dest: grunt.option('buildFolder')
            },
            deploy: {
                expand: true,
                deleteEmptyFolders: true,
                cwd: grunt.option('buildFolder'),
                src: ['**'],
                dest: grunt.option('deployFolder')
            }
        },
        //        concat: {
        //            js: {
        //                src: grunt.option('buildFolder')+'/Content/scripts/**/*.js',
        //                dest: grunt.option('buildFolder')+'/Content/scripts/concat.js'
        //            },
        //            css: {
        //                src: grunt.option('buildFolder')+'/Content/css/**/*.css',
        //                dest: grunt.option('buildFolder')+'/Content/css/concat.css'
        //            }
        //        },
        uglify: {
            js: {
                files: [{
                    src: [grunt.option('buildFolder') + '/Content/scripts/**/*.js'],
                    dest: grunt.option('buildFolder') + '/Content/scripts/concat.min.' + grunt.file.readJSON('package.json').version + '.js'
                }]
            }
        },
        concat: {
            css: {
                src: [grunt.option('buildFolder') + '/Content/css/**/*.css'],
                dest: grunt.option('buildFolder') + '/Content/css/concat.min.css'
            }
        },
        cssmin: {
            minify: {
                src: [grunt.option('buildFolder') + '/Content/css/concat.min.css'],
                dest: 'concat.min.version.css'
            }
        },

        hbsconfigpoke: {
            compile: {
                options: {
                    context: grunt.option(grunt.option('target')),
                },
                files: {
                    [grunt.option('buildFolder') + "/Web.config"]: grunt.option('srcFolder') + '/Web.config.hbs'//,
                    //                    '<%=buildFolder%>/views/shared/_JavascriptDebugFalse.cshtml': grunt.option('srcFolder')+'/views/shared/_javascriptDebugFalse.hbs',
                    //                    '<%=buildFolder%>/views/shared/_CssScriptsDebugFalse.cshtml': grunt.option('srcFolder')+'/views/shared/_CssScriptstDebugFalse.hbs'
                }
            }
        },

        compress: {
            main: {
                options: {
                    archive: `${grunt.option('buildFolder') }/MethodFitness_PROD_${new Date().getFullYear()}_${new Date().getMonth()+1}_${new Date().getDate()}-${new Date().getHours()}_${new Date().getMinutes()}.zip`
                },
                expand: true,
                cwd: grunt.option('buildFolder'),
                src: ['**/*']
            }
        },
        deleteEmptyFolders: {
           src: grunt.option('buildFolder')
        }
    });

    grunt.registerTask('logStart', 'start time and build params.', function() {
        grunt.log.writeln('grunt build started at: '+ grunt.option('startTime').toLocaleTimeString());
        grunt.log.writeln(grunt.option('buildFolder'));
        grunt.log.writeln(grunt.option('solutionName'));
        grunt.log.writeln(grunt.option('srcFolder'));
        grunt.log.writeln(grunt.option('buildConfig'));
        grunt.log.writeln(grunt.option('target'));
        grunt.log.writeln(grunt.option('slnFile'));
    });

    grunt.registerTask('logEnd', 'End time and build params.', function() {
        grunt.log.writeln('grunt build ended at: '+ new Date().toLocaleTimeString());
        var time = new Date(Math.abs(new Date() - grunt.option('startTime')));
        grunt.log.writeln('Duration: '+ ('0' + time.getMinutes()).slice(-2) +':'+ ('0' + time.getSeconds()).slice(-2));
    });
    

    grunt.loadTasks( "./grunt-tasks");
    grunt.loadNpmTasks('grunt-contrib-clean');
    grunt.loadNpmTasks('grunt-msbuild');
    grunt.loadNpmTasks('grunt-contrib-copy');
    grunt.loadNpmTasks('grunt-cleanempty');
    grunt.loadNpmTasks('grunt-hbs-configpoke');
    grunt.loadNpmTasks('grunt-contrib-uglify');
    grunt.loadNpmTasks('grunt-contrib-concat');
    grunt.loadNpmTasks('grunt-contrib-cssmin');
    grunt.loadNpmTasks('grunt-contrib-compress');

    grunt.registerTask('default', ['logStart', 'clean:build', 'msbuild', 'copy:buildArtifacts', 'uglify', 'hbsconfigpoke','compress','logEnd']);
    grunt.registerTask('deploy', ['logStart', 'clean:build', 'msbuild', 'copy:buildArtifacts', 'deleteEmptyFolders', 'hbsconfigpoke', 'compress']);
//     grunt.registerTask('deploy', ['logStart', 'clean:build', 'msbuild', 'copy:buildArtifacts', 'hbsconfigpoke','clean:build','copy:deploy','compress','logEnd']);
};
