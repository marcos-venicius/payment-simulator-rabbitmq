import { useEffect, useState } from "react";
import { api } from "./services/api";

type LogState = "processing" | "success" | "fail" | "retrying";

interface ILog {
  id: string;
  name: string;
  state: LogState;
  price: number;
  createdAt: string;
}

async function loadLogs() {
  const payments = await api.get<ILog[]>("/payments");

  return payments.data;
}

function maskMoney(value: number) {
  return new Intl.NumberFormat("pt-BR", {
    currency: "BRL",
    style: "currency",
  }).format(value);
}

function App() {
  const [logs, setLogs] = useState<ILog[]>([]);
  const [name, setName] = useState("");
  const [price, setPrice] = useState("");

  async function makePayment() {
    if (name.trim().length > 0 && !!Number(price)) {
      await api.post("/payments", {
        name,
        price,
      });

      setName("");
      setPrice("");
    }
  }

  useEffect(() => {
    let timer: NodeJS.Timeout | null = null;

    async function load() {
      const newLogsList = await loadLogs();

      setLogs(newLogsList);

      timer = setTimeout(load, 1000);
    }

    load();

    return () => {
      if (timer) clearTimeout(timer);
    };
  }, []);

  return (
    <main className="container">
      <h1 className="title">Logs de pagamentos</h1>
      <div className="inputs">
        <input
          type="text"
          value={name}
          onChange={(e) => setName(e.target.value)}
          placeholder="name"
        />
        <input
          type="number"
          value={price}
          onChange={(e) => setPrice(e.target.value.replace(/\D/g, ""))}
          placeholder="price"
        />
        <button onClick={makePayment}>pay</button>
      </div>
      <p className="info">processing 3 at time</p>

      <ul className="logs">
        {logs.map((log) => (
          <li className="log" key={log.id}>
            <div className="header">
              <p className="description">
                <span>{maskMoney(log.price)}</span>
                {log.name}
              </p>
              <span className="state" data-state={log.state}>
                {log.state}
              </span>
            </div>
            <p className="date">{new Date(log.createdAt).toString()}</p>
          </li>
        ))}
      </ul>
    </main>
  );
}

export default App;
