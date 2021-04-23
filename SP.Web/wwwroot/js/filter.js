$(document).ready(function () {
    if ($('#RegionList').length) $('#RegionList').chosen().change(changeRegionHandler);
    if ($('#TerritoryList').length) $('#TerritoryList').chosen().change(changeTerritoryHandler);
    if ($('#GasStationList').length) $('#GasStationList').chosen().change(changeStationHandler);
    if ($('#NomenclatureGroupList').length) $('#NomenclatureGroupList').chosen().change(changeNomenclatureGroupHandler);
    if ($('#NomenclatureList').length) $('#NomenclatureList').chosen().change(changeNomenclatureHandler);
});

function storeFilterState() {
    storeFilter('regionFilter', $('#RegionList'));
    storeFilter('terrFilter', $('#TerritoryList'));
    storeFilter('stationFilter', $('#GasStationList'));
    storeFilter('groupFilter', $('#NomenclatureGroupList'));
    storeFilter('nomFilter', $('#NomenclatureList'));
}

function storeFilter(name, control) {
    if ($(control).length) {
        console.log(name, $(control).val().join());
        localStorage.setItem(name, $(control).val().join());
        return;
    }
}

function restoreFilterState() {
    return new Promise(function (resolve, reject) {
        // TODO: восстановить сохранение списков выбора
        resolve(false);
        return;

        var regions = localStorage.getItem('regionFilter');
        var terrs = localStorage.getItem('terrFilter');
        var stations = localStorage.getItem('stationFilter');
        var groups = localStorage.getItem('groupFilter');
        var noms = localStorage.getItem('nomFilter');
        if (!regions && !groups) {
            resolve(false);
            return;
        }

        var regionList = $('#RegionList');
        var nomGroupList = $('#NomenclatureGroupList');
        regionList.chosen().change(null);
        selectOptions(regionList, regions);
        regionList.trigger("chosen:updated");
        changeRegion(terrs, true)
            .then(function () {
                return changeTerritory(stations, true);
            })
            .then(function() {
                regionList.chosen().change(changeRegionHandler);
                nomGroupList.chosen().change(null);
                selectOptions(nomGroupList, groups);
                nomGroupList.trigger("chosen:updated");
                return changeNomenclatureGroup(noms, true);
            })
            .then(function () {
                nomGroupList.chosen().change(changeNomenclatureGroupHandler);
                console.log('finishing restore');
                resolve(true);
            });
    });
}

function changeRegionHandler(e, args) {
    changeRegion();
}

function changeTerritoryHandler(e, args) {
    changeTerritory();
}

function changeStationHandler(e, args) {
    storeFilterState();
}

function changeRegion(terrs, preventChangeTrigger) {
    return new Promise(function (resolve, reject) {
        var terrList = $('#TerritoryList');
        var selectedRegions = $('#RegionList').val();
        var selectedTerrs;
        if (terrs) {
            selectedTerrs = terrs;
        } else {
            selectedTerrs = terrList.val().join();
        }
        if (selectedRegions.length === 0) {
            terrList.empty();
            terrList.trigger("chosen:updated");
            if (!preventChangeTrigger) terrList.trigger('change');
            storeFilterState();
            resolve(true);
            return;
        }
        var url = '/Region/LoadTerritories';
        var requestData = { regions: selectedRegions.join() };
        $.get(url,
            requestData,
            function (data) {
                terrList.empty();
                $.each(data,
                    function (index, element) {
                        terrList.append($('<option>',
                            {
                                value: element.id,
                                text: element.name
                            }));
                    });

                selectOptions(terrList, selectedTerrs);
                terrList.trigger("chosen:updated");
                if (!preventChangeTrigger) terrList.trigger('change');
                storeFilterState();
                resolve(true);
            });
        console.log('finishing changeRegion');
    });
}

function changeTerritory(stations, preventChangeTrigger) {
    return new Promise(function (resolve, reject) {
        var regions = $('#RegionList').val();
        var terrs = $('#TerritoryList').val();
        var stationList = $("#GasStationList");
        var selectedStations;
        if (stations) {
            selectedStations = stations;
        } else {
            selectedStations = stationList.val().join();
        }
        if (terrs.length === 0) {
            stationList.empty();
            stationList.trigger("chosen:updated");
            if (!preventChangeTrigger) stationList.trigger('change');
            storeFilterState();
            resolve(true);
            return;
        }
        var url = '/Station/LoadStations';
        var requestData = {
            'regions': regions.join(),
            'terrs': terrs.join()
        };
        $.post(url,
            requestData,
            function (data) {
                stationList.empty();
                $.each(data,
                    function (index, element) {
                        stationList.append($('<option>',
                            {
                                value: element.id,
                                text: element.name
                            }));
                    });

                selectOptions(stationList, selectedStations);
                stationList.trigger("chosen:updated");
                if (!preventChangeTrigger) stationList.trigger('change');
                storeFilterState();
                resolve(true);
            });
        console.log('finishing changeTerritory');
    });
}

function changeNomenclatureGroupHandler(e, args) {
    changeNomenclatureGroup();
}

function changeNomenclatureHandler(e, args) {
    storeFilterState();
}

function changeNomenclatureGroup(items, preventChangeTrigger) {
    return new Promise(function(resolve, reject) {
        var nomList = $('#NomenclatureList');
        var selectedGroups = $('#NomenclatureGroupList').val();
        var selectedNoms;
        if (items) {
            selectedNoms = items;
        } else {
            selectedNoms = nomList.val().join();
        }
        if (selectedGroups.length === 0) {
            nomList.empty();
            nomList.trigger("chosen:updated");
            if (!preventChangeTrigger) nomList.trigger('change');
            storeFilterState();
            resolve(true);
            return;
        }
        var url = '/Nomenclature/LoadNomenclature';
        var requestData = { groups: selectedGroups.join() };
        $.get(url,
            requestData,
            function(data) {
                nomList.empty();
                $.each(data,
                    function(index, element) {
                        nomList.append($('<option>',
                            {
                                value: element.id,
                                text: element.name
                            }));
                    });

                selectOptions(nomList, selectedNoms);
                nomList.trigger("chosen:updated");
                if (!preventChangeTrigger) nomList.trigger('change');
                storeFilterState();
                resolve(true);
            });
    });
}

function chosenSelectAll(controlId) {
    $('#' + controlId + ' option').prop('selected', true);
    $('#' + controlId).trigger("chosen:updated");
    $('#' + controlId).trigger("change");
}

function chosenDeselectAll(controlId) {
    $('#' + controlId + ' option').prop('selected', false);
    $('#' + controlId).trigger("chosen:updated");
    $('#' + controlId).trigger("change");
}

function selectOptions(control, values) {
    if (!values || values.length === 0) {
        $(control).val('');
        return;
    }
    var selected = values.split(',');
    $(control).val(selected);
}
