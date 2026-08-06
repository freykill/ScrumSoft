// Configuracion de Karma. Ver https://karma-runner.github.io/6.4/config/configuration-file.html
//
// ChromeHeadlessCI existe para el contenedor y el CI: ahi Chrome corre como
// root y sin --no-sandbox se cae al arrancar.

module.exports = function (config) {
    config.set({
        basePath: '',
        frameworks: ['jasmine', '@angular-devkit/build-angular'],
        plugins: [
            require('karma-jasmine'),
            require('karma-chrome-launcher'),
            require('karma-jasmine-html-reporter'),
            require('karma-coverage'),
            require('@angular-devkit/build-angular/plugins/karma')
        ],
        client: {
            jasmine: {},
            // Deja el reporte en el navegador al terminar en modo watch
            clearContext: false
        },
        jasmineHtmlReporter: {
            suppressAll: true
        },
        coverageReporter: {
            dir: require('path').join(__dirname, './coverage/sakai-ng'),
            subdir: '.',
            reporters: [
                { type: 'html' },
                { type: 'text-summary' }
            ]
        },
        reporters: ['progress', 'kjhtml'],
        browsers: ['Chrome'],
        customLaunchers: {
            ChromeHeadlessCI: {
                base: 'ChromeHeadless',
                flags: ['--no-sandbox', '--disable-gpu']
            }
        },
        restartOnFileChange: true
    });
};
