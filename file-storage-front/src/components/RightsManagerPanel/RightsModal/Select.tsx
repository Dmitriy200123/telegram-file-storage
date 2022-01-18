import React, {memo, useState} from 'react';
import "./Select.scss";
import {OutsideAlerter} from "../../utils/OutSideAlerter/OutSideAlerter";

//todo: fix updates redraw
type Props = {
    name: string,
    className?: string, values: any, oldValues: any, setValue: any, options: Array<{ value: any, label: string }> | undefined,
    placeholder?: string, isMulti?: boolean
}

export const Select: React.FC<Props> = memo(({
                                                 className,
                                                 values,
                                                 oldValues,
                                                 setValue,
                                                 options,
                                                 placeholder
                                             }) => {
    if (placeholder === undefined || placeholder === null)
        placeholder = `Доступно: ${values.length}`;
    const [isOpen, changeOpen] = useState(false);
    const [text, changeText] = useState("");
    const regexp = new RegExp(`${text}`, 'i');
    const ui = options?.filter((elem) => !!elem.label.match(regexp))
        .map((elem) => {
            const onChange = () => {
                if (values?.includes(elem.value)) {
                    setValue(values.filter((v: string) => v !== elem.value));
                } else if (values) {
                    setValue([...values, elem.value])
                } else {
                    setValue([elem.value])
                }
            }

            function calcClass() {
                if (oldValues.includes(elem.value)) {
                    return (values.includes(elem.value) ? " rights-modal__option_active" : " rights-modal__option_removed")
                } else {
                    return (values.includes(elem.value) ? " rights-modal__option_added" : "")
                }
            }

            return <li key={elem.value}
                       className={"select__option " + calcClass()}
                       onClick={onChange}>{elem.label}</li>;
        })

    return (
        <OutsideAlerter className={className} onOutsideClick={() => {
            changeOpen(false);
        }}>
            <div className={"select"}>
                <input className={"select__field"} onClick={() => changeOpen(!isOpen)} value={text} onChange={(e) => {
                    changeText(e.target.value)
                }} placeholder={placeholder}/>
                <ul className={"select__list " + (isOpen ? "select__list_open" : "")} onBlur={() => changeOpen(false)}>
                    <div className="select__scroll">
                        {ui}
                    </div>
                </ul>
            </div>
        </OutsideAlerter>
    )
})