// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Initializes a DataTable for an EntityTable-based list page. Keeps the table's own
// horizontal scroll wrapper (.table-responsive) separate from the length/search/info/
// pagination controls, so their focus rings and dropdowns aren't clipped by that
// wrapper's overflow.
function initEntityTable(tableId, options) {
    return $('#' + tableId).DataTable($.extend({
        dom: "<'row'<'col-sm-12 col-md-6'l><'col-sm-12 col-md-6'f>>" +
             "<'row'<'col-sm-12'<'table-responsive't>>>" +
             "<'row'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7'p>>"
    }, options));
}
