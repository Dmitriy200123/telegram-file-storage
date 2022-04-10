import React, {FC, useState} from 'react';
import {Button} from "../../../utils/Button/Button";
import {InputText} from "../../../utils/Inputs/Text/InputText";
import classes from "../Modal.module.scss";

type PropsType = {
    onOutsideClick?: () => void
    value: string,
}


const Edit: FC<PropsType> = ({onOutsideClick, value}) => {
    const [name, changeName] = useState(value);

    function onChange(e: any) {
        changeName(e.target.value);
    }


    return (
        <div className={classes.block}>
            <h2 className={classes.title}>Переименовать</h2>
            <InputText value={name} onChange={onChange}/>
            <div className={classes.btns}>
                <Button type={"transparent"} onClick={onOutsideClick}>ОТМЕНА</Button>
                <Button onClick={undefined}>ОК</Button>
            </div>
        </div>
    );
};


export default Edit;
