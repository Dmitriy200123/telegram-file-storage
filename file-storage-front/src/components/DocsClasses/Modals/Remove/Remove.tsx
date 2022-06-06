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
            <h2 className={classes.title}>Подтверждение</h2>
            <p>Вы действительно ходите удалить классификацию ?</p>
            <div className={classes.btns}>
                <Button onClick={onSubmit}>ОК</Button>
                <Button type={"transparent"} onClick={onOutsideClick}>ОТМЕНА</Button>
            </div>
        </div>
    );
};


export default Remove;
