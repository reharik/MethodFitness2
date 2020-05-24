module.exports = function(grunt) {

    grunt.option('destFolder','build_artifacts');
    grunt.option('solutionName','MF.ClientAbsenteeReport');
    grunt.option('projectName','MF.ClientAbsenteeReport');
    grunt.option('buildConfig',grunt.option('buildConfig') || 'Debug');
    grunt.option('srcFolder', '../../Projects/'+grunt.option('projectName')+'/');
    grunt.option('srcTarget', '../../Projects/'+grunt.option('projectName')+'/bin/'+grunt.option('buildConfig')+'/');
    grunt.option('target',grunt.option('target') || 'QA');
    grunt.option('slnFile','../../Solutions/'+grunt.option('solutionName')+'/'+grunt.option('solutionName')+'.sln');
    grunt.option('deployFolder','/MethodFitness/Services/MF.ClientAbsenteeReport_'+grunt.option('target'));
    grunt.option('outputConfigFile',grunt.option('destFolder')+'/'+grunt.option('projectName')+'.exe.config');
    grunt.option('startTime',new Date());

    grunt.option( 'QA',{
        connection_String:'Server=cannibalcoder.cloudapp.net;Database=MethodFitness_QA;User ID=methodFitness;Password=m3th0d;Connection Timeout=30;',
        environment:grunt.option('target'),
        customErrors:"Off",
        debug:"false",
        AdminEmail:"info.prov@methodfit.com",
        EmailReportAddress:"methodfit@gmail.com"
    });

    grunt.option( 'PROD',{
        connection_String:'Server=localhost;Database=MethodFitness_PROD;User ID=methodFitness;Password=m3th0df1t;Connection Timeout=30;',
        environment:grunt.option('target'),
        customErrors:"RemoteOnly",
        debug:"false",
        EmailReportAddress:"harik.raif@gmail.com",
        AdminEmail:"info.prov@methodfit.com"
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
                cwd:grunt.option('srcTarget'),
                src: ['**',
                    '!**/obj/**',
                    '!**/*.cs',
                    '!**/*.vb',
                    '!**/*.csproj',
                    '!**/*.csproj.*',
                    '!App.config',
                    '!App.config.hbs',
                    '!*.exe.config',
                    '!**/*.xml',
                    '!**/*.pdb'
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

        hbsconfigpoke:{
            compile:{
                options:{
                    context:grunt.option(grunt.option('target'))
                },
                files:{
                    'build_artifacts/MF.ClientAbsenteeReport.exe.config' : grunt.option('srcFolder') + '/App.config.hbs'
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
        grunt.log.writeln(grunt.option('outputConfigFile'));
        grunt.log.writeln(grunt.option('srcTarget'));
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

    grunt.registerTask('default', ['logStart', 'clean', 'msbuild', 'copy:buildArtifacts', 'cleanempty', 'hbsconfigpoke','logEnd']);
    grunt.registerTask('deploy', ['logStart', 'clean', 'msbuild', 'copy:buildArtifacts', 'cleanempty', 'hbsconfigpoke','copy:deploy','logEnd']);
};
