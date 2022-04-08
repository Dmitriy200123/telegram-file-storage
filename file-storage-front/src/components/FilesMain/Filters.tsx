import React, {FC, memo, useState} from "react";
import {Select} from "../utils/Inputs/Select";
import {SelectTime} from "../utils/Inputs/SelectDate";
import {ReactComponent as Search} from "../../assets/search.svg";
import {ReactComponent as Cross} from "../../assets/close.svg";
import {ReactComponent as Settings} from "../../assets/settings-icon.svg";
import {configFilters} from "./ConfigFilters";
import {useAppSelector} from "../../utils/hooks/reduxHooks";
import {DefaultValues, KeepStateOptions, UseFormGetValues} from "react-hook-form";
import {TypeSelectFilters} from "./FilesMain";
import {SelectText} from "../utils/Inputs/SelectText";

type TypeProps = {
    setValueForm: (name: string, value: any) => void,
    getValues: UseFormGetValues<TypeSelectFilters>,
    reset: (values?: DefaultValues<TypeSelectFilters>, keepStateOptions?: KeepStateOptions) => void
}

export const Filters: FC<TypeProps> = memo(({setValueForm, getValues, reset}) => {
    const [isOpen, changeIsOpen] = useState(false);
    const filesNames = useAppSelector((state) => state.filesReducer.filesNames);
    const chats = useAppSelector((state) => state.filesReducer.chats);
    const senders = useAppSelector((state) => state.filesReducer.senders);
    const filesTypes = useAppSelector((state) => state.filesReducer.filesTypes);
    const {optionsName, optionsSender, optionsChat} = configFilters(filesNames, chats, senders);
    const optionsCategory = filesTypes && Object.keys(filesTypes).map((key) => ({label: filesTypes[key], value: key}));
    const options: Array<{ label: string, value: any }> = [];

    function onReset() {
        reset({
            date: {
                dateTo: null,
                dateFrom: null
            }
        }, {
            keepDirty: true,
        });
    }

    const valuesCategories = getValues("categories");
    return <div className={"files__filters"}>
        <div className={"files__filters-main"}>
            <button className={"files__btn-search"} type="submit"><Search/></button>
            <SelectText name={"fileName"} className={"files__filter files__filter_select"}
                        setValue={setValueForm}
                        value={getValues("fileName") || ""}
                        placeholder={isOpen ? "Поиск файла по названию или слову содержащийся в нём" : "Поиск файла по названию"}
                        options={optionsName} isMulti={false}/>
            <button className={"files__btn-open-filter"} type="button" onClick={() => changeIsOpen(!isOpen)}>
                <Settings/>
                <span>Фильтр</span>
                {isOpen && <Cross className={"files__btn-open-filter-close"}/>}

            </button>
        </div>
        {isOpen && <div className={"files__filters-add"}>
            <div className={"files__filters-add-block"}>
                <SelectTime name={"date"} className={"files__filter files__filter_select"}
                            setValue={setValueForm}
                            values={getValues("date")} placeholder={"Выберите дату"}/>
                <Select name={"senderIds"} className={"files__filter files__filter_select"}
                        setValue={setValueForm} placeholder={"Отправитель"}
                        values={getValues("senderIds")} options={optionsSender} isMulti={true}/>
                <Select name={"chatIds"} className={"files__filter files__filter_select"}
                        setValue={setValueForm} placeholder={"Чат"}
                        values={getValues("chatIds")} options={optionsChat} isMulti={true}/>
                <Select name={"categories"} className={"files__filter files__filter_select"}
                        setValue={setValueForm} placeholder={"Формат"}
                        values={valuesCategories} options={optionsCategory} isMulti={true}/>


                {!!valuesCategories?.length && (valuesCategories.includes('6') && valuesCategories.length === 1) && <>
                    <Select name={"1"} className={"files__filter files__filter_select"}
                            setValue={setValueForm} placeholder={"Категория"}
                            values={null} options={options} isMulti={true}/>
                </>}
            </div>
            <button className={"files__reset"} type="reset" onClick={onReset}>
                <Cross/>
                <span>Сбросить фильтры</span>
            </button>
        </div>}
    </div>
})
