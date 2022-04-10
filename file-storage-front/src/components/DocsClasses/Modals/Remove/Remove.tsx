import React, {FC} from 'react';
import {Button} from "../../../utils/Button/Button";
import classes from "../Modal.module.scss";

type PropsType = {
    onOutsideClick?: () => void
}
const Remove: FC<PropsType> = ({onOutsideClick}) => {

    return (
        <div className={classes.block}>
            <h2 className={classes.title}>Точно удалить ?</h2>
            <div className={classes.btns}>
                <Button type={"transparent"} onClick={onOutsideClick}>ОТМЕНА</Button>
                <Button onClick={undefined}>ОК</Button>
            </div>
        </div>
    );
};


export default Remove;
