import { faPencil, faTrash } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { useNavigate } from "react-router-dom";
import Button from "../../../components/common/buttons/_button";

const ActionButton = ({ handleDelete, item }) => {
    const navigate = useNavigate();

    function handleUpdate(item) {
        navigate(`update/${item.id}`, {
            state: { data: item }
        })
    }

    return (
        <>
            <td className="px-6 py-4 text-center">
                <Button onClick={() => handleDelete(item)} type="icon" className="text-center font-medium text-blue-600 hover:underline">
                    <FontAwesomeIcon icon={faTrash} />
                </Button>
            </td>
            <td className="px-6 py-4 text-center">
                <Button onClick={() => handleUpdate(item)} type="icon" className="text-center font-medium text-blue-600 hover:underline">
                    <FontAwesomeIcon icon={faPencil} />
                </Button>
            </td>
        </>
    );
}
export default ActionButton 