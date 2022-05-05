import React, {FC} from 'react';
import {Button} from "../../../utils/Button/Button";
import classes from "../Modal.module.scss";
import {fetchDeleteClassification} from "../../../../redux/classesDocs/classesDocsThunks";
import {useAppDispatch} from "../../../../utils/hooks/reduxHooks";
import {useHistory} from "react-router-dom";

type PropsType = {
    onOutsideClick?: () => void
    args: {id:string}
}
const Remove: FC<PropsType> = ({onOutsideClick, args}) => {
    const dispatch = useAppDispatch();
    const history = useHistory();

    function onSubmit() {
        dispatch(fetchDeleteClassification({ id: args.id}));
        onOutsideClick?.();
        if (history.location.pathname !== "docs-сlasses")
            history.push("/docs-сlasses");
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
