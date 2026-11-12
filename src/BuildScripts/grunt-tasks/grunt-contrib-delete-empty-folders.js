const fs = require('fs');
module.exports = function (grunt) {
	grunt.registerMultiTask('deleteEmptyFolders', 'Deletes the empty folders.', function () {
		console.log(JSON.stringify(this, null,2))
		let recur = (path) => {
			if (!fs.statSync(path).isDirectory()) {
				return;
			}
			const dir = fs.readdirSync(path);
			if (dir.length === 0) {
				fs.rmdirSync(path);
				return;
			}

			dir.filter(item => fs.statSync(`${path}/${item}`).isDirectory())
				.forEach(dir => recur(`${path}/${dir}`));
			if (fs.readdirSync(path).length === 0) {
				fs.rmdirSync(path);
			}
		}
		recur(this.filesSrc[0]);
	});

}