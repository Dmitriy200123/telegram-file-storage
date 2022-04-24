import React, {FC} from 'react';
import {Button} from "../../../utils/Button/Button";
import classes from "../Modal.module.scss";
import {fetchDeleteClassification, fetchRenameClassification} from "../../../../redux/classesDocs/classesDocsThunks";
import {fetchRemoveFile} from "../../../../redux/thunks/fileThunks";
import {useAppDispatch} from "../../../../utils/hooks/reduxHooks";

type PropsType = {
    onOutsideClick?: () => void
    args: {id:string}
}
const Remove: FC<PropsType> = ({onOutsideClick, args}) => {
    const dispatch = useAppDispatch();

    function onSubmit() {
        dispatch(fetchDeleteClassification({ id: args.id}));
        onOutsideClick?.();
    }

    return (
        <div className={classes.block}>
            <h2 className={classes.title}>Точно удалить ?</h2>
            <div className={classes.btns}>
                <Button type={"transparent"} onClick={onOutsideClick}>ОТМЕНА</Button>
                <Button onClick={onSubmit}>ОК</Button>
            </div>
        </div>
    );
};


export default Remove;
