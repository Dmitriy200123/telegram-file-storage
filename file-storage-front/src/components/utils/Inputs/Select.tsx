import React, {useState} from 'react';
import {Category} from "../../../models/File";
import "./Select.scss";

const optionsCategory: Array<{ value: Category, label: Category }> = [
    {value: 'images', label: 'images'},
    {value: 'links', label: 'links'},
    {value: 'video', label: 'video'},
    {value: 'documents', label: 'documents'},
];

type Props = {name: string, onChangeForm: any, className?:string, register: any, getValues: any, setValue: any }

const Select: React.FC<Props> = ({name, onChangeForm, className, register, getValues, setValue }) => {
    const [isOpen, changeOpen] = useState(false);

    const ui = optionsCategory.map((elem) => {
        const onChange = () => {
            const values = getValues(name);
            if (values.includes(elem.value)) {
                setValue(name, values.filter((v: Category) => v !== elem.value));
            } else {
                setValue(name, [...values, elem.value])
            }

            onChangeForm();
        };

        const values = getValues(name);
        return <li
            className={"select__option " + (values && (values.includes(elem.value)) ? "select__option_active" : "")}
            onClick={onChange}>{elem.label}</li>;
    })


    return (
        <div className={className}>
            <div style={{padding: 100}} className={"select"}>
                {getValues(name)}
                <div className={"select__field"} onClick={() => changeOpen(!isOpen)}>Тыкни сюда</div>
                <ul className={"select__list " + (isOpen ? "select__list_open" : "")}>
                    {ui}
                </ul>
                <select multiple={true} {...register(name, {onChange: () => onChangeForm})}/>
            </div>
        </div>
    )
}

export default Select;
