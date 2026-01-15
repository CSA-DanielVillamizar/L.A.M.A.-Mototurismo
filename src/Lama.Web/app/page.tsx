export default function HomePage() {
  return (
    <div className="max-w-4xl mx-auto px-4 py-12">
      <div className="text-center">
        <h1 className="text-4xl font-bold text-gray-900 mb-4">
          Bienvenido al Sistema COR de LAMA
        </h1>
        <p className="text-lg text-gray-600 mb-8">
          Sistema de gestión de evidencias y confirmación de asistencias
        </p>

        <div className="bg-white rounded-lg shadow-md p-8 text-left">
          <h2 className="text-2xl font-semibold text-gray-800 mb-4">
            Funcionalidades Disponibles
          </h2>

          <div className="space-y-4">
            <div className="border-l-4 border-primary-500 pl-4">
              <h3 className="font-semibold text-gray-900">Subir Evidencia</h3>
              <p className="text-gray-600">
                Permite a los MTOs subir evidencias fotográficas (piloto con moto + odómetro)
                y confirmar asistencias de miembros a eventos.
              </p>
              <a
                href="/evidence/upload"
                className="inline-block mt-2 text-primary-600 hover:text-primary-700 font-medium"
              >
                Ir a Subir Evidencia →
              </a>
            </div>

            <div className="border-l-4 border-gray-300 pl-4">
              <h3 className="font-semibold text-gray-900">API Backend</h3>
              <p className="text-gray-600">
                Conectado a API .NET 8 con SQL Server. Validación automática de STATUS (33 valores),
                cálculo de puntos por evento y distancia.
              </p>
            </div>
          </div>
        </div>

        <div className="mt-8 text-sm text-gray-500">
          <p>Versión: 1.0.0 | Next.js 14 + TypeScript + Tailwind CSS</p>
        </div>
      </div>
    </div>
  );
}
