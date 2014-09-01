module.exports = function(grunt) {

    grunt.option('destFolder','build_artifacts');
    grunt.option('projectName','MF.RoundHouse');
    grunt.option('srcFolder', '../../Projects/'+grunt.option('projectName')+'/');
    grunt.option('target',grunt.option('target') || 'QA');
    grunt.option('startTime',new Date());

    grunt.option( 'QA',{
        ConnectionString:'Server=cannibalcoder.cloudapp.net;Database=MethodFitness_QA;User ID=methodFitness;Password=m3th0d;Connection Timeout=30;'
    });

    grunt.option( 'PROD',{
        ConnectionString:'Server=cannibalcoder.cloudapp.net;Database=MethodFitness_PROD;User ID=methodFitness;Password=m3th0d;Connection Timeout=30;'
    });

    grunt.initConfig({
        pkg: grunt.file.readJSON('package.json'),
        destFolder: grunt.option('destFolder'),

// tasks

        roundhouse:{
            run:{
                options:{
                    RH:'../../Solutions/lib/roundhouse/rh.exe',
                    connstring:grunt.option(grunt.option('target')).ConnectionString,
                    output:grunt.option('srcFolder')+'Output',
                    versionfile:'package.json',
                    sqlfilesdirectory:grunt.option('srcFolder')+'MethodFit/',
                    withtransaction:'true'
                }
            }
        },

        bump: {
            options: {
                files: ['package.json'],
                updateConfigs: ['pkg'],
                commit: grunt.option('target').toLowerCase() == 'prod',
                commitMessage: 'Release v%VERSION%',
                commitFiles: ['package.json'], // '-a' for all files
                createTag: true,
                tagName: grunt.option('projectName') + ' v%VERSION%',
                tagMessage: grunt.option('projectName') + ' Version %VERSION%',
                push: grunt.option('target').toLowerCase() == 'prod',
                pushTo: 'origin production',
                gitDescribeOptions: '--tags --always --abbrev=1 --dirty=-d' // options to use with '$ git describe'

            }
        }
    });

    grunt.registerTask('logStart', 'start time and build params.', function() {
        grunt.log.writeln('grunt build started at: '+ grunt.option('startTime').toLocaleTimeString());
        grunt.log.writeln('destFolder: '+ grunt.option('destFolder'));
        grunt.log.writeln('projectName: '+grunt.option('projectName'));
        grunt.log.writeln('srcFolder: '+ grunt.option('srcFolder'));
        grunt.log.writeln('target: '+grunt.option('target'));
    });

    grunt.registerTask('logEnd', 'End time and build params.', function() {
        grunt.log.writeln('grunt build ended at: '+ new Date().toLocaleTimeString());
        var time = new Date(Math.abs(new Date() - grunt.option('startTime')));
        grunt.log.writeln('Duration: '+ ('0' + time.getMinutes()).slice(-2) +':'+ ('0' + time.getSeconds()).slice(-2));
    });

    grunt.registerTask('addOriginToGit', 'adding remote to branch if it doesnt exist.', function() {
        if (grunt.option('target').toLowerCase() != 'prod') { return; }
        var exec = require('child_process').exec;
        var done = grunt.task.current.async(); // Tells Grunt that an async task is complete
        exec('git ls-remote || git remote add origin git@github.com:reharik/MethodFitness.git', function (err, stdout, stderr) {
            if (err) {
                grunt.fatal('Can not create the commit:\n  ' + stderr);
            }
            done(err);
        });
    });

    grunt.loadNpmTasks('grunt-bump');
    grunt.loadNpmTasks('grunt-roundhouse');

    grunt.registerTask('default', ['logStart', 'roundhouse','logEnd']);
    grunt.registerTask('deploy', ['logStart', 'roundhouse','logEnd']);
    grunt.registerTask('deploy_prod', ['logStart', 'roundhouse', 'addOriginToGit', 'bump', 'logEnd']);
};
