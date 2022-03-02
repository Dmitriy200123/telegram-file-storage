import React, {memo, useState} from 'react';
import "./Select.scss";
import {OutsideAlerter} from "../OutSideAlerter/OutSideAlerter";

//todo: fix updates redraw
type Props = {
    name: string,
    className?: string,
    values: any,
    setValue: (name: string, value: any) => void, options: Array<{ value: any, label: string }> | undefined,
    placeholder?: string, isMulti?: boolean
}
export const Select: React.FC<Props> = memo(({
                                                 name,
                                                 className,
                                                 values,
                                                 setValue,
                                                 options,
                                                 placeholder,
                                                 isMulti
                                             }) => {
    if (placeholder === undefined || placeholder === null)
        placeholder = "Введите текст";
    const [isOpen, changeOpen] = useState(false);
    const [text, changeText] = useState("");
    const regexp = new RegExp(`${text}`, 'i');
    const ui = options?.filter((elem) => !!elem.label.match(regexp))
        .map((elem) => {
            const onChange = () => {
                if (isMulti) {
                    if (values?.includes(elem.value)) {
                        setValue(name, values.filter((v: string) => v !== elem.value));
                    } else if (values) {
                        setValue(name, [...values, elem.value])
                    } else {
                        setValue(name, [elem.value])
                    }
                } else {
                    if (values === elem.value)
                        setValue(name, null)
                    else
                        setValue(name, elem.value)
                }

            }
            const isActive = (isMulti ? (values?.includes(elem.value)) : (values === elem.value));
            return <li key={elem.value}
                       className={"select__option " + (isActive ? "select__option_active" : "")}
                       onClick={onChange}>{elem.label}</li>;
        })

    return (
        <OutsideAlerter className={className} onOutsideClick={() => {
            changeOpen(false);
        }}>
            <div className={"select"}>
                <input className={"select__field"} onClick={() => changeOpen(!isOpen)} value={text} onChange={(e) => {
                    changeText(e.target.value)
                }} placeholder={(options && calcPlaceholder(values, options)) ?? placeholder}/>
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