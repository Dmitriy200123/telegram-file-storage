import React, {ChangeEvent, memo, useState} from 'react';
import "./Select.scss";
import {OutsideAlerter} from "../OutSideAlerter/OutSideAlerter";

//todo: fix updates redraw
type Props = {
    name: string,
    className?: string,
    value: string,
    setValue: (name: string, value: any) => void, options: Array<{ value: any, label: string }> | undefined,
    placeholder?: string,
    isMulti?: boolean,

}
export const SelectText: React.FC<Props> = memo(({
                                                     name,
                                                     className,
                                                     value,
                                                     setValue,
                                                     options,
                                                     placeholder,
                                                 }) => {

    if (placeholder === undefined || placeholder === null)
        placeholder = "Поиск файла по названию"

    const [isOpen, changeOpen] = useState(false);
    const regexp = new RegExp(`${value}`, 'i');
    const ui = options?.filter((elem) => !!elem.label.match(regexp))
        .map((elem) => {
            const onChange = () => {
                if (value === elem.value)
                    setValue(name, null)
                else
                    setValue(name, elem.value)
            }
            const isActive = (value === elem.value);
            return <li key={elem.value}
                       className={"select__option " + (isActive ? "select__option_active" : "")}
                       onClick={onChange}>{elem.label}</li>;
        })

    function onChange(e: ChangeEvent<HTMLInputElement>) {
        const value = e.target.value;
        setValue(name, value)
    }

    return (
        <OutsideAlerter className={className} onOutsideClick={() => {
            changeOpen(false);
        }}>
            <div className={"select"}>
                <input className={"select__field"} onClick={() => changeOpen(!isOpen)} value={value}
                       onChange={onChange}
                       placeholder={(options && calcPlaceholder(value, options)) ?? placeholder}/>
                <ul className={"select__list " + (isOpen ? "select__list_open" : "")} onBlur={() => changeOpen(false)}>
                    <div className="select__scroll">
                        {ui}
                    </div>
                </ul>
            </div>
        </OutsideAlerter>
    )
})

const calcPlaceholder = (values: Array<string | number> | string, options: Array<{ value: any, label: string }>) => {
    if (values instanceof Array) {
        return values.length === 0 ? null
            : values.map(e => options.find((opt) => opt.value === e)?.label).join(", ");
    } else {
        return options?.find(e => e.value === values)?.label;
    }
}