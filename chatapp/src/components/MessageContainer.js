const MessageContainer = ({ messages }) => {
    if (!messages) return null; // Eğer messages undefined veya null ise, hiçbir şey render etmeyin

    return (
        <div>
            {
                messages.map((message, index) => (
                    <table className="table table-striped table-bordered" key={index}>
                        <tbody>
                            <tr>
                                <td>{message.message} - {message.username}</td>
                            </tr>
                        </tbody>
                    </table>
                ))
            }
        </div>
    );
}

export default MessageContainer;
