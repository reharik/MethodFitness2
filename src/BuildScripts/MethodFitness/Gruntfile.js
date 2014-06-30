module.exports = function(grunt) {

    grunt.option('destFolder','build_artifacts');
    grunt.option('solutionName','MethodFitness');
    grunt.option('projectName','MF.Web');
    grunt.option('srcFolder', '../../Projects/'+grunt.option('projectName')+'/');
    grunt.option('buildConfig',grunt.option('buildConfig') || 'Debug');
    grunt.option('target',grunt.option('target') || 'QA');
    grunt.option('slnFile','../../Solutions/'+grunt.option('solutionName')+'/'+grunt.option('solutionName')+'.sln');
    grunt.option('deployFolder','/WebSites/MethodFitness_'+grunt.option('target'));
    grunt.option('startTime',new Date());

    grunt.option( 'QA',{
        connection_String:'Server=cannibalcoder.cloudapp.net;Database=MethodFitness_QA;User ID=methodFitness;Password=m3th0d;Connection Timeout=30;',
        host_Name:'http://mfqa.methodfit.net',
        admin_Email:'methodfit_qa@methodfit.com'
    });

    grunt.option( 'PROD',{
           connection_String:'Server=cannibalcoder.cloudapp.net;Database=MethodFitness_PROD;User ID=methodFitness;Password=m3th0d;Connection Timeout=30;',
           host_Name:'http://methodfit.net',
           admin_Email:'methodfit@methodfit.com'
    });

    grunt.initConfig({
        pkg: grunt.file.readJSON('package.json'),
        destFolder: grunt.option('destFolder'),

// tasks
        clean: {
            build: [grunt.option('destFolder')]
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
                deleteEmptyFolders:true,
                cwd:grunt.option('srcFolder'),
                src: ['**',
                    '!**/obj/**',
                    '!**/*.cs',
                    '!**/*.vb',
                    '!**/*.csproj',
                    '!**/*.csproj.*',
                    '!Web.config',
                    '!Web.config.hbs'

                ],
                dest: grunt.option('destFolder')
            },
            deploy:{
                expand: true,
                deleteEmptyFolders:true,
                cwd:grunt.option('destFolder'),
                src: ['**'],
                dest: grunt.option('deployFolder')
            }
        },
//        concat: {
//            js: {
//                src: grunt.option('destFolder')+'/Content/scripts/**/*.js',
//                dest: grunt.option('destFolder')+'/Content/scripts/concat.js'
//            },
//            css: {
//                src: grunt.option('destFolder')+'/Content/css/**/*.css',
//                dest: grunt.option('destFolder')+'/Content/css/concat.css'
//            }
//        },
        uglify: {
            my_target: {
                files: [{
                    src:  grunt.option('destFolder')+'/Content/scripts/**/*.js',
                    dest:  grunt.option('destFolder')+'/Content/scripts/concat.min.js'
                }]
            }
        },

        hbsconfigpoke:{
            compile:{
                options:{
                    context:grunt.option(grunt.option('target'))
                },
                files:{
                    '<%=destFolder%>/Web.config': grunt.option('srcFolder')+'/Web.config.hbs'
                }
            }
        },

        cleanempty: {
            options: {},
            src: [grunt.option('destFolder')+'/**']
        }
    });

    grunt.registerTask('logStart', 'start time and build params.', function() {
        grunt.log.writeln('grunt build started at: '+ grunt.option('startTime').toLocaleTimeString());
        grunt.log.writeln(grunt.option('destFolder'));
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

    grunt.loadNpmTasks('grunt-contrib-clean');
    grunt.loadNpmTasks('grunt-msbuild');
    grunt.loadNpmTasks('grunt-contrib-copy');
    grunt.loadNpmTasks('grunt-cleanempty');
    grunt.loadNpmTasks('grunt-hbs-configpoke');
    grunt.loadNpmTasks('grunt-contrib-concat');
    grunt.loadNpmTasks('grunt-contrib-uglify');

    grunt.registerTask('default', ['logStart', 'clean', 'msbuild', 'copy:buildArtifacts', 'uglify', 'cleanempty', 'hbsconfigpoke','logEnd']);
    grunt.registerTask('deploy', ['logStart', 'clean', 'msbuild', 'copy:buildArtifacts', 'cleanempty', 'hbsconfigpoke','copy:deploy','logEnd']);
};
