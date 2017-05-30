
$(document).ready(function () {

    // set the default edit language to the current langauge
    $('#editlang').val($('#selectparams #lang').val());

    // get list of records via ajax:  NBrightRazorTemplate_nbxget({command}, {div of data passed to server}, {return html to this div} )
    NBrightBuyDepot_nbxget('getdata', '#selectparams', '#editdata');

    $('.actionbuttonwrapper #cmdsave').unbind('click');
    $('.actionbuttonwrapper #cmdsave').click(function () {
        NBrightBuyDepot_nbxget('savedata', '#editdata');
    });

    $('.actionbuttonwrapper #cmdreturn').unbind('click');
    $('.actionbuttonwrapper #cmdreturn').click(function () {
        $('#selecteditemid').val(''); // clear sleecteditemid.        
        NBrightBuyDepot_nbxget('getdata', '#selectparams', '#editdata');
    });

    $('.actionbuttonwrapper #cmddelete').unbind('click');
    $('.actionbuttonwrapper #cmddelete').click(function () {
        if (confirm($('#deletemsg').val())) {
            NBrightBuyDepot_nbxget('deleterecord', '#editdata');
        }
    });

    $('#addnew').unbind('click');
    $('#addnew').click(function () {
        $('.processing').show();
        $('#newitem').val('new');
        $('#selecteditemid').val('');
        NBrightBuyDepot_nbxget('addnew', '#selectparams', '#editdata');
    });

    $('.selecteditlanguage').unbind('click');
    $('.selecteditlanguage').click(function () {
        $('#editlang').val($(this).attr('lang')); // alter lang after, so we get correct data record
        NBrightBuyDepot_nbxget('selectlang', '#editdata'); // do ajax call to save current edit form
    });

    //-------------------------------------------------------
    // -------------  CLIENT SELECT    ----------------------
    //-------------------------------------------------------

    $('#clientselectlist').unbind('change');
    $('#clientselectlist').change(function () {
        // select product
        $('.selectclient').unbind();
        $('.selectclient').click(function () {
            $('.selectuserid' + $(this).attr('itemid')).hide();
            $('input[id*="selecteduserid"]').val($(this).attr('itemid'));
            NBrightBuyDepot_nbxget('addproductclient', '#productselectparams', '#productclients'); // load releated
        });
    });


    // select search
    $('#clientlistsearch').unbind('click');
    $('#clientlistsearch').click(function () {
        $('input[id*="searchtext"]').val($('input[id*="txtClientSearch"]').val());
        NBrightBuyDepot_nbxget('getclientselectlist', '#productselectparams', '#clientselectlist');
    });

    $('#clientselect').unbind('click');
    $('#clientselect').click(function () {
        $(this).hide();
        $("input[id*='header']").val("clientselectheader.html");
        $("input[id*='body']").val("clientselectbody.html");
        $("input[id*='footer']").val("clientselectfooter.html");
        $('.depotdetail').hide();
        $('#clientselectsection').show();
    });

    $('#returnfromclientselect').unbind('click');
    $('#returnfromclientselect').click(function () {
        $('#clientselect').show();
        $("input[id*='header']").val("productlistheader.html");
        $("input[id*='body']").val("productlistbody.html");
        $("input[id*='footer']").val("productlistfooter.html");
        $("input[id*='searchtext']").val('');
        NBrightBuyDepot_nbxget('productclients', '#productdata', '#productclients');
        $('#clientselectsection').hide();
        $('#productdatasection').show();
    });

    $('#productclients').unbind('change');
    $('#productclients').change(function () {
        $('.removeclient').click(function () {
            $('input[id*="selecteduserid"]').val($(this).attr('itemid'));
            NBrightBuyDepot_nbxget('removeproductclient', '#productselectparams', '#productclients');
        });
    });

    //-------------------------------------------------------
    // CLIENT SELECT END: -----------------------------------
    //-------------------------------------------------------


});

$(document).on("NBrightBuyDepot_nbxgetcompleted", NBrightBuyDepot_nbxgetCompleted); // assign a completed event for the ajax calls


function NBrightBuyDepot_nbxget(cmd, selformdiv, target, selformitemdiv, appendreturn) {
    $('.processing').show();

    $.ajaxSetup({ cache: false });

    var cmdupdate = '/DesktopModules/NBright/NBrightBuyDepot/XmlConnector.ashx?cmd=' + cmd;
    var values = '';
    if (selformitemdiv == null) {
        values = $.fn.genxmlajax(selformdiv);
    }
    else {
        values = $.fn.genxmlajaxitems(selformdiv, selformitemdiv);
    }
    var request = $.ajax({
        type: "POST",
        url: cmdupdate,
        cache: false,
        data: { inputxml: encodeURI(values) }
    });

    request.done(function (data) {
        if (data != 'noaction') {
            if (appendreturn == null) {
                $(target).children().remove();
                $(target).html(data).trigger('change');
            } else
                $(target).append(data).trigger('change');

            $.event.trigger({
                type: "NBrightBuyDepot_nbxgetcompleted",
                cmd: cmd
            });
        }
        if (cmd == 'getdata') { // only hide on getdata
            $('.processing').hide();
        }
    });

    request.fail(function (jqXHR, textStatus) {
        alert("Request failed: " + textStatus);
    });
}



function NBrightBuyDepot_nbxgetCompleted(e) {

    if (e.cmd == 'addnew') {
        $('#newitem').val(''); // clear item so if new was just created we don;t create another record
        $('#selecteditemid').val($('#itemid').val()); // move the itemid into the selecteditemid, so page knows what itemid is being edited
        NBrightBuyDepot_DetailButtons();
        $('.processing').hide(); // hide on add, not hidden by return
    }

    if (e.cmd == 'deleterecord') {
        $('#selecteditemid').val(''); // clear selecteditemid, it now doesn;t exists.
        NBrightBuyDepot_nbxget('getdata', '#selectparams', '#editdata');// relist after delete
    }

    if (e.cmd == 'savedata') {
        $('#selecteditemid').val(''); // clear sleecteditemid.        
        NBrightBuyDepot_nbxget('getdata', '#selectparams', '#editdata');// relist after save
    }

    if (e.cmd == 'selectlang') {
        NBrightBuyDepot_nbxget('getdata', '#selectparams', '#editdata'); // do ajax call to get edit form
    }

    // check if we are displaying a list or the detail and do processing.
    if (($('#selecteditemid').val() != '') || (e.cmd == 'addnew')) {
        // PROCESS DETAIL
        NBrightBuyDepot_DetailButtons();

        if ($('input:radio[name=typeselectradio]:checked').val() == "cat") {
            $('.catdisplay').show();
            $('.propdisplay').hide();
        } else {
            $('.catdisplay').hide();
            $('.propdisplay').show();
        }

        $('input:radio[name=typeselectradio]').unbind('change');
        $('input:radio[name=typeselectradio]').change(function () {
            if ($(this).val() == 'cat') {
                $('.catdisplay').show();
                $('.propdisplay').hide();
            } else {
                $('.catdisplay').hide();
                $('.propdisplay').show();
            }
        });

        if ($('input:radio[name=applydiscounttoradio]:checked').val() == "1") {
            $('.applyproperty').hide();
        } else {
            $('.applyproperty').show();
        }

        $('input:radio[name=applydiscounttoradio]').unbind('change');
        $('input:radio[name=applydiscounttoradio]').change(function () {
            if ($(this).val() == '1') {
                $('.applyproperty').hide();
            } else {
                $('.applyproperty').show();
            }
        });

        if ($('.applydaterangechk').is(":checked")) {
            $('.applydaterange').show();
        }

        $('.applydaterangechk').unbind('change');
        $('.applydaterangechk').change(function () {
            if ($(this).is(":checked")) {
                $('.applydaterange').show();
            } else {
                $('.applydaterange').hide();
            }
        });

        $('#cmdrecalcpromo').unbind('click');
        $('#cmdrecalcpromo').click(function () {
            if (confirm($('#deletemsg').val())) {
                $('.processing').show();
                NBrightBuyDepot_nbxget('recalc', '#selectparams', '#editdata'); // do ajax call to get edit form
            }
        });

    } else {
        //PROCESS LIST
        NBrightBuyDepot_ListButtons();
        $('.edititem').unbind('click');
        $('.edititem').click(function () {
            $('.processing').show();
            $('#selecteditemid').val($(this).attr("itemid")); // assign the sleected itemid, so the server knows what item is being edited
            NBrightBuyDepot_nbxget('getdata', '#selectparams', '#editdata'); // do ajax call to get edit form
        });
        $(".catdisplay").prop("disabled", true);
        $(".propdisplay").prop("disabled", true);
    }



}

function NBrightBuyDepot_DetailButtons() {
    $('#cmdsave').show();
    $('#cmddelete').show();
    $('#cmdreturn').show();
    $('#addnew').hide();
    $('input[datatype="date"]').datepicker(); // assign datepicker to any ajax loaded fields
}

function NBrightBuyDepot_ListButtons() {
    $('#cmdsave').hide();
    $('#cmddelete').hide();
    $('#cmdreturn').hide();
    $('#addnew').show();
}


