'use client';

import React, { useState, useEffect } from 'react';
import { LayoutWrapper } from '@/components/layout';
import { Card } from '@/components/ui/card';
import { Badge } from '@/components/ui/badge';
import { Skeleton } from '@/components/ui/skeleton';
import { IconUpload } from '@/components/icons';
import { cn } from '@/lib/utils';

/**
 * Interfaz de patrocinador
 */
interface Sponsor {
  id: string;
  name: string;
  logo: string;
  website: string;
  description: string;
  category: 'motos' | 'accesorios' | 'servicios' | 'alojamiento' | 'otros';
  benefits: string[];
  discountPercentage?: number;
  featured?: boolean;
}

/**
 * Componente de Patrocinadores
 * Muestra lista de patrocinadores oficiales con beneficios para miembros
 */
export function Sponsors() {
  const [isLoading, setIsLoading] = useState(true);
  const [sponsors, setSponsors] = useState<Sponsor[]>([]);
  const [selectedCategory, setSelectedCategory] = useState<string>('todos');

  useEffect(() => {
    // Simular carga de datos
    const timer = setTimeout(() => {
      const mockData: Sponsor[] = [
        {
          id: 'sponsor-1',
          name: 'Harley-Davidson Colombia',
          logo: 'https://images.unsplash.com/photo-1558618666-fcd25c85cd64?w=200&h=200&fit=crop',
          website: 'https://www.harley-davidson.com.co',
          description:
            'Fabricante oficial de motocicletas de peso medio a pesado. Patrocinador principal de eventos nacionales.',
          category: 'motos',
          benefits: [
            '15% descuento en accesorios',
            'Promociones exclusivas en modelo nuevo',
            'Servicio técnico prioritario',
            'Invitaciones a eventos especiales',
          ],
          discountPercentage: 15,
          featured: true,
        },
        {
          id: 'sponsor-2',
          name: 'Yamaha Andina',
          logo: 'https://images.unsplash.com/photo-1607818814316-f0d6b44e3284?w=200&h=200&fit=crop',
          website: 'https://www.yamaha.com.co',
          description:
            'Fabricante de motocicletas deportivas y de aventura. Aliado oficial de L.A.M.A.',
          category: 'motos',
          benefits: [
            '12% descuento en repuestos',
            'Revisiones técnicas gratuitas',
            'Acceso a cursos de conducción segura',
            'Seguro especial para miembros',
          ],
          discountPercentage: 12,
          featured: true,
        },
        {
          id: 'sponsor-3',
          name: 'RevMoto Accesorios',
          logo: 'https://images.unsplash.com/photo-1521575107034-e3fb11b08e77?w=200&h=200&fit=crop',
          website: 'https://www.revmoto.com',
          description:
            'Distribuidor de accesorios y repuestos para motocicletas de todas las marcas.',
          category: 'accesorios',
          benefits: [
            '20% descuento en todos los productos',
            'Envío gratis para pedidos mayores a $200.000',
            'Asesoramiento técnico personalizado',
            'Programa de lealtad con puntos',
          ],
          discountPercentage: 20,
        },
        {
          id: 'sponsor-4',
          name: 'Cascos SeguriRide',
          logo: 'https://images.unsplash.com/photo-1591768696164-0f1c0a60e2f8?w=200&h=200&fit=crop',
          website: 'https://www.seguriride.com',
          description: 'Fabricante de cascos de seguridad homologados internacionalmente.',
          category: 'accesorios',
          benefits: [
            '25% descuento en toda la línea',
            'Garantía extendida de 3 años',
            'Servicio de revisión gratuita',
          ],
          discountPercentage: 25,
          featured: true,
        },
        {
          id: 'sponsor-5',
          name: 'Mecánica Express',
          logo: 'https://images.unsplash.com/photo-1486376146528-5d6e44e1a4a8?w=200&h=200&fit=crop',
          website: 'https://www.mecanicaexpress.com',
          description:
            'Taller especializado en mantenimiento y reparación de motocicletas.',
          category: 'servicios',
          benefits: [
            '10% descuento en servicios',
            'Revisión preventiva anual sin costo',
            'Servicio a domicilio disponible',
            'Atención prioritaria',
          ],
          discountPercentage: 10,
        },
        {
          id: 'sponsor-6',
          name: 'Hotel Moto Adventure',
          logo: 'https://images.unsplash.com/photo-1566073771259-6a8506099945?w=200&h=200&fit=crop',
          website: 'https://www.motoadventure.com',
          description:
            'Cadena hotelera especializada en hospedaje para mototuristas con estacionamientos seguros.',
          category: 'alojamiento',
          benefits: [
            '30% descuento en habitaciones',
            'Desayuno incluido para miembros Premium',
            'Estacionamiento vigilado 24/7',
            'Paquetes de fin de semana especiales',
          ],
          discountPercentage: 30,
          featured: true,
        },
        {
          id: 'sponsor-7',
          name: 'Combustibles PetroMoto',
          logo: 'https://images.unsplash.com/photo-1553531088-f350cfdd3997?w=200&h=200&fit=crop',
          website: 'https://www.petromoto.com',
          description: 'Estaciones de servicio con beneficios especiales para mototuristas.',
          category: 'servicios',
          benefits: [
            '5% descuento en combustible',
            'Programa de puntos dobles para miembros',
            'Estaciones limpias y bien mantenidas',
            'Conveniencia 24/7',
          ],
          discountPercentage: 5,
        },
        {
          id: 'sponsor-8',
          name: 'Seguros MotoTrack',
          logo: 'https://images.unsplash.com/photo-1454165804606-c3d57bc86b40?w=200&h=200&fit=crop',
          website: 'https://www.mototrack.com',
          description: 'Pólizas de seguros especializadas para motocicletas y mototurismo.',
          category: 'servicios',
          benefits: [
            'Cotización especial para miembros',
            'Descuentos del 15-40% según plan',
            'Atención 24/7 en ruta',
            'Cobertura internacional',
          ],
          discountPercentage: 15,
        },
      ];

      setSponsors(mockData);
      setIsLoading(false);
    }, 800);

    return () => clearTimeout(timer);
  }, []);

  const categories = [
    { id: 'todos', label: 'Todos', count: sponsors.length },
    {
      id: 'motos',
      label: 'Motos',
      count: sponsors.filter((s) => s.category === 'motos').length,
    },
    {
      id: 'accesorios',
      label: 'Accesorios',
      count: sponsors.filter((s) => s.category === 'accesorios').length,
    },
    {
      id: 'servicios',
      label: 'Servicios',
      count: sponsors.filter((s) => s.category === 'servicios').length,
    },
    {
      id: 'alojamiento',
      label: 'Alojamiento',
      count: sponsors.filter((s) => s.category === 'alojamiento').length,
    },
  ];

  const filteredSponsors = sponsors.filter(
    (sponsor) =>
      selectedCategory === 'todos' || sponsor.category === selectedCategory
  );

  const featuredSponsors = filteredSponsors.filter((s) => s.featured);
  const otherSponsors = filteredSponsors.filter((s) => !s.featured);

  if (isLoading) {
    return (
      <LayoutWrapper
        title="Patrocinadores"
        breadcrumbs={[
          { label: 'Inicio', href: '/' },
          { label: 'Patrocinadores', href: '/sponsors' },
        ]}
      >
        <div className="space-y-6">
          {[1, 2, 3].map((i) => (
            <Skeleton key={i} className="h-32 w-full" />
          ))}
        </div>
      </LayoutWrapper>
    );
  }

  return (
    <LayoutWrapper
      title="Patrocinadores Oficiales"
      breadcrumbs={[
        { label: 'Inicio', href: '/' },
        { label: 'Patrocinadores', href: '/sponsors' },
      ]}
    >
      <div className="space-y-6">
        {/* Introducción */}
        <Card className="p-6 bg-gradient-to-r from-primary-50 to-secondary-50">
          <h2 className="text-xl font-semibold text-neutral-900 mb-2">
            Beneficios Exclusivos para Miembros
          </h2>
          <p className="text-neutral-700">
            L.A.M.A. ha establecido alianzas estratégicas con las mejores marcas
            del mototurismo. Como miembro, accede a descuentos y beneficios
            especiales en todos nuestros patrocinadores.
          </p>
        </Card>

        {/* Filtros por Categoría */}
        <div className="flex gap-2 overflow-x-auto pb-2">
          {categories.map((category) => (
            <button
              key={category.id}
              onClick={() => setSelectedCategory(category.id)}
              className={cn(
                'px-4 py-2 rounded-full font-medium whitespace-nowrap transition-colors',
                selectedCategory === category.id
                  ? 'bg-primary-600 text-white'
                  : 'bg-neutral-100 text-neutral-700 hover:bg-neutral-200'
              )}
            >
              {category.label} ({category.count})
            </button>
          ))}
        </div>

        {/* Patrocinadores Destacados */}
        {featuredSponsors.length > 0 && (
          <div>
            <h3 className="text-lg font-bold text-neutral-900 mb-4">
              ⭐ Patrocinadores Destacados
            </h3>
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
              {featuredSponsors.map((sponsor) => (
                <SponsorCard key={sponsor.id} sponsor={sponsor} />
              ))}
            </div>
          </div>
        )}

        {/* Otros Patrocinadores */}
        {otherSponsors.length > 0 && (
          <div>
            <h3 className="text-lg font-bold text-neutral-900 mb-4">
              Otros Patrocinadores
            </h3>
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
              {otherSponsors.map((sponsor) => (
                <SponsorCard key={sponsor.id} sponsor={sponsor} />
              ))}
            </div>
          </div>
        )}
      </div>
    </LayoutWrapper>
  );
}

/**
 * Tarjeta individual de patrocinador
 */
function SponsorCard({ sponsor }: { sponsor: Sponsor }) {
  return (
    <Card className="overflow-hidden hover:shadow-lg transition-shadow">
      {/* Logo */}
      <div className="h-32 bg-neutral-100 flex items-center justify-center overflow-hidden">
        <img
          src={sponsor.logo}
          alt={sponsor.name}
          className="w-full h-full object-cover"
        />
      </div>

      {/* Contenido */}
      <div className="p-4">
        <div className="flex items-start justify-between gap-2 mb-2">
          <h3 className="font-bold text-neutral-900">{sponsor.name}</h3>
          {sponsor.featured && (
            <Badge className="bg-warning-100 text-warning-700 text-xs flex-shrink-0">
              Destacado
            </Badge>
          )}
        </div>

        <p className="text-sm text-neutral-600 mb-3">{sponsor.description}</p>

        {/* Descuento */}
        {sponsor.discountPercentage && (
          <div className="mb-3 p-2 bg-success-50 rounded-lg">
            <p className="text-lg font-bold text-success-700">
              {sponsor.discountPercentage}% descuento
            </p>
          </div>
        )}

        {/* Beneficios */}
        <div className="mb-4">
          <p className="text-xs font-semibold text-neutral-700 mb-2 uppercase">
            Beneficios
          </p>
          <ul className="space-y-1">
            {sponsor.benefits.slice(0, 2).map((benefit, idx) => (
              <li key={idx} className="text-xs text-neutral-600">
                ✓ {benefit}
              </li>
            ))}
            {sponsor.benefits.length > 2 && (
              <li className="text-xs text-primary-600 font-medium">
                +{sponsor.benefits.length - 2} beneficios más
              </li>
            )}
          </ul>
        </div>

        {/* Botón */}
        <a
          href={sponsor.website}
          target="_blank"
          rel="noopener noreferrer"
          className="w-full py-2 px-3 bg-primary-600 hover:bg-primary-700 text-white text-sm font-medium rounded-lg transition-colors text-center"
        >
          Visitar Sitio
        </a>
      </div>
    </Card>
  );
}
