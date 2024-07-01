import { Col, Container, Row } from 'react-bootstrap';
import './App.css';
import 'bootstrap/dist/css/bootstrap.min.css';
import WaitingRoom from './components/waitingroom';
import { useState } from 'react';
import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import ChatRoom from './components/ChatRoom';

function App() {
  const[connection, setConnection] = useState();
  const[messages, setMessages] = useState([]);

  const joinChatRoom = async (username, chatroom) => {
    try {
      const connection = new HubConnectionBuilder()
                          .withUrl("http://localhost:5212/chat")
                          .configureLogging(LogLevel.Information)
                          .build();

      connection.on("JoinSpecificChatRoom", (username, message) => {
        console.log("message: ", message);
      });

      connection.on("ReceiveSpesificMessage", (username, message) => {
        setMessages(messages => [...messages, {username, message}]);
      });

      await connection.start();
      await connection.invoke("JoinSpecificChatRoom", {username, chatroom});

      setConnection(connection);

    } catch (e) {
      console.log(e);
    }
  }

  const sendMessage = async(message) => {
    try {
      await connection.invoke("SendMessage", message);
    } catch (e) {
      console.log(e);
    }
  }

  return (
    <div>
      <main>
        <Container>
          <Row class='px-5 my-5'>
            <Col sm='12'>
              <h1 className='font-weight-light'>Welcome to the ChatApp</h1>
            </Col>
          </Row>
          {!connection ?
          <WaitingRoom joinChatRoom={joinChatRoom}></WaitingRoom>
          : <ChatRoom messages={messages} sendMessage={sendMessage}></ChatRoom>
          }
        </Container>
      </main>
    </div>
  );
}

export default App;
