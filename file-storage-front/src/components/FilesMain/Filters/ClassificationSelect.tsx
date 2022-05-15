import React, {FC} from "react";
import {Select} from "../../utils/Inputs/Select";
import {useAppSelector} from "../../../utils/hooks/reduxHooks";

type PropsType = {
    setValueForm: (name: string, value: any) => void,
    values: string[] | null
}

const ClassificationFilter: FC<PropsType> = ({setValueForm, values}) => {
    const classifications = useAppSelector((state) => state.classesDocs.classifications);
    const options = classifications?.map((e) => ({value: e.id, label: e.name}))
    return (<Select name={"1"} className={"files__filter files__filter_select"}
                    setValue={setValueForm} placeholder={"Классификация"}
                    values={values} options={options} isMulti={true}/>)
}

export default ClassificationFilter;