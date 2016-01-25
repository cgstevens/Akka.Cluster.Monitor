/**
 * Copyright (c) 2011-2013 Felix Gnass
 * Licensed under the MIT license
 */

/*

Basic Usage:
============

$('#el').spin(); // Creates a default Spinner using the text color of #el.
$('#el').spin({ ... }); // Creates a Spinner using the provided options.

$('#el').spin(false); // Stops and removes the spinner.

Using Presets:
==============

$('#el').spin('small'); // Creates a 'small' Spinner using the text color of #el.
$('#el').spin('large', '#fff'); // Creates a 'large' white Spinner.

Adding a custom preset:
=======================

$.fn.spin.presets.flower = {
  lines: 9
  length: 10
  width: 20
  radius: 0
}

$('#el').spin('flower', 'red');

*/

(function (factory) {

    if (typeof exports == 'object') {
        // CommonJS
        factory(require('jquery'), require('spin'))
    }
    else if (typeof define == 'function' && define.amd) {
        // AMD, register as anonymous module
        define(['jquery', 'spin'], factory)
    }
    else {
        // Browser globals
        if (!window.Spinner) throw new Error('Spin.js not present')
        factory(window.jQuery, window.Spinner)
    }

}(function ($, Spinner) {

    var modal_opts = {
        lines: 11, // The number of lines to draw
        length: 23, // The length of each line
        width: 8, // The line thickness
        radius: 40, // The radius of the inner circle
        corners: 1, // Corner roundness (0..1)
        rotate: 9, // The rotation offset
        color: '#FFF', // #rgb or #rrggbb
        speed: 1, // Rounds per second
        trail: 50, // Afterglow percentage
        shadow: true, // Whether to render a shadow
        hwaccel: false, // Whether to use hardware acceleration
        className: 'spinner', // The CSS class to assign to the spinner
        zIndex: 2e9, // The z-index (defaults to 2000000000)
        top: 'auto', // Top position relative to parent in px
        left: 'auto' // Left position relative to parent in px
    };

    $.fn.spin = function (opts, color) {
        if (opts == "modal") opts = modal_opts;
        return this.each(function () {
            var $this = $(this),
              data = $this.data();

            if (data.spinner) {
                data.spinner.stop();
                delete data.spinner;
                if (opts == modal_opts) {
                    $("#spin_modal_overlay").remove();
                    return;
                }
            }
            if (opts !== false) {
                var spinElem = this;
                if (opts == modal_opts) {
                    $('body').append('<div id="spin_modal_overlay" style="background-color: rgba(0, 0, 0, 0.6); width:100%; height:100%; position:fixed; top:0px; left:0px; z-index:' + (opts.zIndex - 1) + '"/>');
                    spinElem = $("#spin_modal_overlay")[0];
                }

                opts = $.extend(
                  { color: color || $this.css('color') },
                  $.fn.spin.presets[opts] || opts
                )

                data.spinner = new Spinner(opts).spin(spinElem);
            }
        })
    }

    $.fn.spin.presets = {
        tiny: { lines: 8, length: 2, width: 2, radius: 3 },
        small: { lines: 8, length: 4, width: 3, radius: 5 },
        large: { lines: 10, length: 8, width: 4, radius: 8 }
    }

}));